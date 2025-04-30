using System;
using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using Unity.VisualScripting;

namespace Tool.ModularHouseBuilder.SubTool
{
    public class BuildingEditor_Window : EditorWindow
    {
        public static void OpenWindow()
        {
            string artFolderPath = EditorPrefs.GetString("ModularHouseBuilder_ART_FOLDER");

            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{artFolderPath}BuildingEditIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Building Editor", windowIcon, "Tool to edit buildings");

            BuildingEditor_Window window = GetWindow<BuildingEditor_Window>();
            window.titleContent = titleContent;
        }

        private ModulesExplorer_Window _moduleExplorer;
        private ModuleData _selectedModuleData;

        private BuildingData _buildingData;
        private Building _building;

        private Material _previewMaterial_A;
        private Material _previewMaterial_N;

        private Pose _modulePose;
        private float _overlapSizeMultiplier;

        #region ------------ SETUP ROUTINE -------------------------------
        private void OnEnable()
        {
            //Get Building
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
                return;

            if (_moduleExplorer == null)
            {
                //Open Module Displayer 
                _moduleExplorer = ModulesExplorer_Window.OpenWindow();
            }
            _moduleExplorer.ShowModuleInfo(false);
            _moduleExplorer.OnModuleSelected += UpdateSelectedModule;
            _moduleExplorer.OnWindowDestroyed += Close;

            //Get Building
            if (prefabStage == null)
                return;

            _building = prefabStage.prefabContentsRoot.GetComponent<Building>();

            if(_building == null)
                return;

            _buildingData = _building.BuildingData;
            UpdateBuildingData();

            //Load materials
            LoadPreviewMaterial();

            _modulePose = new Pose(Vector3.zero, Quaternion.identity);
            _overlapSizeMultiplier = 0.25f;

            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            _moduleExplorer.OnModuleSelected -= UpdateSelectedModule;
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnFocus()
        {
            if (_building != null)
                return;

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage == null)
                return;

            _building = prefabStage.prefabContentsRoot.GetComponent<Building>();
            _buildingData = _building.BuildingData;

            UpdateBuildingData();
        }
        #endregion


        //Buttons in scene
        //Save asset -> Save asset popup window
        //Remove all scripts (Keep two saves)
        //Delete asset -> Delete asset popo window

        //Show Available assets

        //Highlight modules

        //Enter Prefab Mode
        //Edit only on prefab mode

        //Asset Preview in scene
        //Show overlap box
        //Show snappable moduless

        //Show Saved Modules
        //Show Unsaved Modules

        private void OnGUI()
        {
            _overlapSizeMultiplier = EditorGUILayout.FloatField("Extents Multiplier", _overlapSizeMultiplier, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Update Building Data", GUILayout.ExpandWidth(true)))
                UpdateBuildingData();
        }

        #region ------------ SCENE GUI METHODS -----------------------------
        private void OnSceneGUI(SceneView sceneView)
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if(prefabStage == null)
                return;

            //Check if tool is allowed
            if (prefabStage.prefabContentsRoot.gameObject != _building.gameObject)
                return;

            //Do all only if an object is selected
            if (_selectedModuleData == null)
                return;

            //Reset Module position
            _modulePose.position = Vector3.zero;

            //Get Scene Camera
            Camera sceneCamera = sceneView.camera;

            //Get Collision Point With Plane
            Plane worldPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            //Raycast on World Plane
            if (worldPlane.Raycast(ray, out float enter))
            {
                Vector3 collisionPoint = ray.GetPoint(enter);

                //Update Module Position
                _modulePose.position = collisionPoint;

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

                //Overlap and ALL
                Vector3 boxPosition = _modulePose.position;
                boxPosition.y += _selectedModuleData.CenterPoint.y;

                Quaternion boxRotation = _modulePose.rotation;
                Vector3 boxExtention = _selectedModuleData.Extension;
                
                List<HouseModule> overlappingModules = new List<HouseModule>();
                List<HouseModule> nearModules = new List<HouseModule>();

                overlappingModules = OverlapBoxAtPoint(boxPosition, boxRotation, boxExtention);
                
                DrawBox(boxPosition, boxRotation, boxExtention, _overlapSizeMultiplier, Color.white);


                //Show Preview
                Material previewMaterial = overlappingModules.Count <= 0 ? _previewMaterial_A : _previewMaterial_N;
                DrawModulePreviewAtPoint(_selectedModuleData, _modulePose, previewMaterial);

                //Repaint Scene View
                SceneView.RepaintAll();
            }


            if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                InstantiateModule(_selectedModuleData);
                Event.current.Use();
            }

            if(Event.current.keyCode == KeyCode.Escape && _selectedModuleData != null)
            {
                _selectedModuleData = null;
            }
        }


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

            DrawBox(position, rotation, overlapExtents, 0f, Color.red);

            //Get Colliding object
            overlapResult = Physics.OverlapBox(position, overlapExtents, rotation);

            //Get Only House Modules
            foreach (Collider col in overlapResult)
            {
                Debug.Log("- "+col.gameObject.name);
                if(col.gameObject.TryGetComponent<HouseModule>(out HouseModule module))
                    modules.Add(module);
            }

            return modules;
        }

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


        private void DrawModulesOutlines(List<HouseModule> modules)
        {
            foreach(HouseModule module in modules)
            {

            }
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

        private void UpdateBuildingData()
        {
            //Update Modules in building
            List<HouseModule> modulesInScene = _building.GetComponentsInChildren<HouseModule>().ToList();
        }
        #endregion
    }
}
