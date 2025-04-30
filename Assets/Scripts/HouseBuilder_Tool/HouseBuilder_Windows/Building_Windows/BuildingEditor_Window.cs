using System;
using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEditor.TerrainTools;

namespace Tool.ModularHouseBuilder.SubTool
{
    public class BuildingEditor_Window : EditorWindow
    {
        private static BuildingEditor_Window Instance = null;
        private static Building s_BuildingToEdit = null;
        private static bool s_UpdateScenes = false;

        public static void OpenWindow(Building buildingToEdit)
        {
            //Update Data To Edit
            s_BuildingToEdit = buildingToEdit;

            //Check if window is already opened
            if(Instance != null)
            {
                s_UpdateScenes = false;
                return;
            }
            else
                s_UpdateScenes = true;

            //Create Window Params
            string artFolderPath = EditorPrefs.GetString("ModularHouseBuilder_ART_FOLDER");

            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{artFolderPath}BuildingEditIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Building Editor", windowIcon, "Tool to edit buildings");

            //Open Window -> Invokes OnEnable
            BuildingEditor_Window window = GetWindow<BuildingEditor_Window>();
            window.titleContent = titleContent;
            Instance = window;
        }

        private ModulesExplorer_Window _moduleExplorer;
        private ModuleData _selectedModuleData;

        private BuildingData _buildingData;
        private Building _building;

        private Material _previewMaterial_A;
        private Material _previewMaterial_N;

        private Pose _modulePose;
        private float _overlapSizeMultiplier;

        private Scene _tempBuildingScene;
        private Scene[] _enteringScenes;
        private Scene _enteringActiveScene;

        private bool _allowOverlap = false;

        #region ------------ UNITY RUNTIME -------------------------------
        private void OnEnable()
        {
            _building = s_BuildingToEdit;
            _buildingData = s_BuildingToEdit.BuildingData;

            if (s_UpdateScenes)
                UpdateOpenedScenes();

            OpenTemporaryScene();
            
            SceneView.duringSceneGui += OnSceneGUI;
            Application.quitting += OnApplicationQuit;

            if (_moduleExplorer == null)
            {
                //Open Module Displayer 
                _moduleExplorer = ModulesExplorer_Window.OpenWindow();
            }
            _moduleExplorer.ShowModuleInfo(false);
            _moduleExplorer.OnModuleSelected += UpdateSelectedModule;
            _moduleExplorer.OnWindowDestroyed += Close;

            //Load materials
            LoadPreviewMaterial();

            _modulePose = new Pose(Vector3.zero, Quaternion.identity);
            _overlapSizeMultiplier = 0.25f;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            _moduleExplorer.OnModuleSelected -= UpdateSelectedModule;
        }

        private void OnDestroy()
        {
            Instance = null;
            s_BuildingToEdit = null;
            s_UpdateScenes = true;

            OpenPreviousScenes();
        }

        //Before Quitting Open last scenes
        private void OnApplicationQuit() => OpenPreviousScenes();
        #endregion

        #region ------------ SCENE MANAGEMENT -------------------------------
        //Since editor is in a temporary scene that will be closed -> Save previous scenes
        private void UpdateOpenedScenes()
        {
            //Get Active Scene
            _enteringActiveScene = SceneManager.GetActiveScene();

            //Get all scenes
            int scenesOpenedCount = SceneManager.sceneCount;
            _enteringScenes = new Scene[scenesOpenedCount];
            for (int i = 0; i < scenesOpenedCount; i++)
                _enteringScenes[i] = SceneManager.GetSceneAt(i);
        }

        private void OpenPreviousScenes()
        {
            CleanScene();

            //Load All Scenes
            foreach (Scene scene in _enteringScenes)
                EditorSceneManager.OpenScene(scene.name, OpenSceneMode.Additive);
            //Set Active Scene
            SceneManager.SetActiveScene(_enteringActiveScene);

            //Close temp scene
            EditorSceneManager.CloseScene(_tempBuildingScene, true);
        }

        private void OpenTemporaryScene()
        {
            _tempBuildingScene = EditorSceneManager.OpenScene("Assets/Scripts/HouseBuilder_Tool/TempScene.unity", OpenSceneMode.Single);
            SceneManager.SetActiveScene(_tempBuildingScene);

            //Instanciate Prefab
            _building = (Building)PrefabUtility.InstantiatePrefab(_building, _tempBuildingScene);
        }

        private void CleanScene()
        {
            //Remove All Game objects
            GameObject[] roots = _tempBuildingScene.GetRootGameObjects();
            foreach(GameObject root in roots)
                DestroyImmediate(root);

            //Save Scene
            EditorSceneManager.SaveOpenScenes();
        }
        #endregion

        #region ------------ TOOL LIFE CYCLE ------------------------------
        private void OnGUI()
        {
            //Buttons in scene
            //Remove all scripts (Keep two saves)
            //Delete asset -> Delete asset popo window

            _overlapSizeMultiplier = EditorGUILayout.FloatField("Extents Multiplier", _overlapSizeMultiplier, GUILayout.ExpandWidth(true));
            
            GUILayout.Space(10f);
            if(GUILayout.Button("Save Building"))
                SaveBuilding();

            GUILayout.Space(10f);
            _allowOverlap = GUILayout.Toggle(_allowOverlap, "Allow Overlap: ");
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            //Highlight modules
            //Asset Preview in scene
            //Show overlap box
            //Show snappable moduless

            //Show Saved Modules
            //Show Unsaved Modules

            //Do all only if an object is selected
            if (_selectedModuleData == null)
                return;

            //Reset Module position
            _modulePose.position = Vector3.zero;

            //Get Scene Camera
            Camera sceneCamera = sceneView.camera;
            
            //Get Point to place module to
            Plane worldPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hitData))
            {
                _modulePose.position = hitData.point;
            }
            //Raycast on World Plane
            else if(worldPlane.Raycast(ray, out float enter))
            {
                Vector3 collisionPoint = ray.GetPoint(enter);
                //Update Module Position
                _modulePose.position = collisionPoint;
            }

            //SHIFT + 
            if (Event.current.shift)
            {
                //Rotate
                if (Event.current.type == EventType.ScrollWheel)
                {
                    float rotationAngles = Event.current.delta.x;
                    _modulePose.rotation.eulerAngles += new Vector3(0f, rotationAngles, 0f);
                    SnapRotation(5f);
                }
            }

            //CONTROL + 
            if (Event.current.control)
            {
                //Snap Position
                SnapModuleToGrid();
            }


            //CHECK COLLISIONS
            Vector3 boxPosition = _modulePose.position;
            boxPosition.y += _selectedModuleData.CenterPoint.y;

            Quaternion boxRotation = _modulePose.rotation;
            Vector3 boxExtention = _selectedModuleData.Extension;

            List<HouseModule> overlappingModules = new List<HouseModule>();
            List<HouseModule> nearModules = new List<HouseModule>();

            //Get modules that are too close to selected module
            DrawBox(boxPosition, boxRotation, boxExtention, 0f, Color.red);
            overlappingModules = OverlapBoxAtPoint(boxPosition, boxRotation, boxExtention, -0.0005f);

            //Analyze Collisions
            if (overlappingModules.Count <= 0 || _allowOverlap)
            {
                //Draw Overlap box with size multiplier
                DrawBox(boxPosition, boxRotation, boxExtention, _overlapSizeMultiplier, Color.white);

                //Get near Modules
                nearModules = OverlapBoxAtPoint(boxPosition, boxRotation, boxExtention, _overlapSizeMultiplier);
                if(nearModules.Count > 0)
                {
                    //draw near modules

                    if(Event.current.control)
                    {
                        //Snap

                    }
                }
            }

            //Show Preview
            Material previewMaterial = overlappingModules.Count <= 0 || _allowOverlap ? _previewMaterial_A : _previewMaterial_N;
            DrawModulePreviewAtPoint(_selectedModuleData, _modulePose, previewMaterial);

            //Repaint Scene View
            SceneView.RepaintAll();


            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {

                if(overlappingModules.Count <= 0 || _allowOverlap)
                {
                    InstantiateModule(_selectedModuleData);
                    Event.current.Use();
                }
            }

            if (Event.current.keyCode == KeyCode.Escape && _selectedModuleData != null)
            {
                _selectedModuleData = null;
            }
        }
        #endregion

        #region------------ DATA UPDATE ----------------------------------
        private void LoadPreviewMaterial()
        {
            string materialAssetPath = EditorPrefs.GetString("ModularHouseBuilder_ART_FOLDER");
            string material_AcceptedName = "PreviewMaterial_A.mat";
            string material_NegatedName = "PreviewMaterial_N.mat";

            _previewMaterial_A = AssetDatabase.LoadAssetAtPath($"{materialAssetPath}{material_AcceptedName}", typeof(Material)) as Material;
            _previewMaterial_N = AssetDatabase.LoadAssetAtPath($"{materialAssetPath}{material_NegatedName}", typeof(Material)) as Material;
        }

        private void UpdateSelectedModule(ModuleData moduleData) => _selectedModuleData = moduleData;
        
        private void SaveBuilding()
        {
            //Record Object
            Undo.RecordObject(_building, "Building Saved");

            //Update Prefab
            Building buildingPrefab = _building;
            PrefabUtility.SaveAsPrefabAsset(buildingPrefab.gameObject, _buildingData.BuildingPath);
            PrefabUtility.RevertPrefabInstance(buildingPrefab.gameObject, InteractionMode.AutomatedAction);

            //Save Scriptable
            EditorUtility.SetDirty(_buildingData);
            AssetDatabase.SaveAssetIfDirty(_buildingData);

            Undo.FlushUndoRecordObjects();
        }
        #endregion

        private void SnapModuleToGrid()
        {
            Vector3 gridSize = EditorSnapSettings.gridSize;
            _modulePose.position = _modulePose.position.Round(gridSize.x);
        }

        private void SnapRotation(float degrees)
        {
            Vector3 roundedRotation = _modulePose.rotation.eulerAngles.Round(degrees);
            _modulePose.rotation.eulerAngles = roundedRotation;
        }

        private void InstantiateModule(ModuleData moduleData)
        {
            Undo.RecordObject(_building, "Building Edited");

            HouseModule module = PrefabUtility.InstantiatePrefab(moduleData.Module).GetComponent<HouseModule>();
            module.transform.parent = _building.transform;

            //Record Object Creation
            Undo.RegisterCreatedObjectUndo(module.gameObject, $"Module {moduleData.ModuleName} created");

            //Adjust module position
            module.transform.position = _modulePose.position;
            module.transform.rotation = _modulePose.rotation;

            //Add Module To Building Data
            _buildingData.AddModule(moduleData);
            EditorUtility.SetDirty(_buildingData);

            Undo.FlushUndoRecordObjects();
        }

        private List<HouseModule> OverlapBoxAtPoint(Vector3 position, Quaternion rotation, Vector3 overlapExtents, float sizeMultiplier = 0f)
        {
            Collider[] overlapResult;
            List<HouseModule> modules = new List<HouseModule>();

            //Adjust overlap size
            overlapExtents += overlapExtents * sizeMultiplier;

            //Get Colliding object
            overlapResult = Physics.OverlapBox(position, overlapExtents/2f, rotation);

            //Get Only House Modules
            foreach (Collider col in overlapResult)
            {
                if (col.gameObject.TryGetComponent<HouseModule>(out HouseModule module))
                    modules.Add(module);
            }

            return modules;
        }

        #region ------------ DRAW METHODS -----------------------------
        private void DrawBox(Vector3 boxPosition, Quaternion boxRotation, Vector3 boxExtents, float sizeMultiplier, Color color)
        {
            boxExtents += boxExtents * sizeMultiplier;

            //Sets Draw Parameters
            Handles.color = color;
            Matrix4x4 handlesMatrix = Matrix4x4.TRS(boxPosition, boxRotation, Vector3.one);
            Handles.matrix = handlesMatrix;

            //Draw Box
            Handles.DrawWireCube(Vector3.zero, boxExtents);
        }

        private void DrawModulePreviewAtPoint(ModuleData moduleToShow, Pose modulePose, Material previewMaterial)
        {
            HouseModule module = moduleToShow.Module;

            MeshFilter[] meshFilters = module.GetComponentsInChildren<MeshFilter>();
            Matrix4x4 positionToWorldMatrix = Matrix4x4.TRS(modulePose.position, modulePose.rotation, Vector3.one);

            foreach (MeshFilter filter in meshFilters)
            {
                Matrix4x4 childLocalMatrix = filter.transform.localToWorldMatrix;
                Matrix4x4 childToWorldMatrix = positionToWorldMatrix * childLocalMatrix;

                previewMaterial.SetPass(0);

                Graphics.DrawMeshNow(filter.sharedMesh, childToWorldMatrix);
            }
        }

        private void DrawModulesOutlines(List<HouseModule> modules)
        {
            foreach (HouseModule module in modules)
            {

            }
        }
        #endregion
    }
}
