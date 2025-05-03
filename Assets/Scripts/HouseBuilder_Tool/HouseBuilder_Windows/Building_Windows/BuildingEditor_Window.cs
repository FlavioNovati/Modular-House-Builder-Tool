using System;
using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

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
        private float _overlapSizeIncreaser;

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
            _overlapSizeIncreaser = 1f;
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

            _overlapSizeIncreaser = EditorGUILayout.FloatField("Extents Multiplier", _overlapSizeIncreaser, GUILayout.ExpandWidth(true));
            
            GUILayout.Space(10f);
            if(GUILayout.Button("Save Building"))
                SaveBuilding();

            GUILayout.Space(10f);
            _allowOverlap = GUILayout.Toggle(_allowOverlap, "Allow Overlap");
        }


        private void OnSceneGUI(SceneView sceneView)
        {
            //Do all only if an object is selected
            if (_selectedModuleData == null)
                return;


            //Get Module Position
            _modulePose.position = GetModulePositionViaRaycasting();

            //Get Near modules
            List<HouseModule> nearModules = GetNearModules(_modulePose, _selectedModuleData.Extension, _overlapSizeIncreaser);
            
            if(Event.current.control)
            {
                if(nearModules.Count > 0)
                {
                    //Snap to module
                    HouseModule closestModule = GetClosestModule(nearModules);
                    _modulePose.position = ApplySnap(_selectedModuleData, closestModule, _modulePose);
                }
                else
                {
                    //Snap to grid
                    _modulePose.position = SnapModuleToGrid(_modulePose.position);
                }
            }
            
            //Get Colliding modules
            List<HouseModule> collidingModules = GetCollidingModules(_modulePose, _selectedModuleData.Extension);

            //Draw
            DrawData(_selectedModuleData, _modulePose, nearModules, collidingModules);

            //Repaint Scene View
            SceneView.RepaintAll();

            //Input handling
            Event currentEvent = Event.current;
            EventType currentEventType = currentEvent.type;

            //Instanciate Module
            if (currentEventType == EventType.MouseDown && currentEvent.button == 0)
            {
                //Instanciate module if allowed
                if (collidingModules.Count <= 0 || _allowOverlap)
                    InstantiateModule(_selectedModuleData);

                currentEvent.Use();
            }

            //Allow unselect module
            if (currentEvent.keyCode == KeyCode.Escape && _selectedModuleData != null)
                _selectedModuleData = null;

            //Module Rotation
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

            //Snap Position
            //if (Event.current.control)
            //    SnapModuleToGrid();
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

        #region------------ PHYSICS -----------------------------------
        private Vector3 GetModulePositionViaRaycasting()
        {
            //Get Point to place module
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData))
            {
                return hitData.point;
            }
            //Raycast on World Plane
            else
            {
                Plane worldPlane = new Plane(Vector3.up, Vector3.zero);
                if (worldPlane.Raycast(ray, out float enter))
                {
                    Vector3 collisionPoint = ray.GetPoint(enter);
                    //Update Module Position
                    return collisionPoint;
                }
            }

            return Vector3.zero;
        }

        private List<HouseModule> GetCollidingModules(Pose pose, Vector3 extension) => OverlapBoxAtPoint(pose.position + (Vector3.up * extension.y / 2), pose.rotation, extension, -0.0005f);
        private List<HouseModule> GetNearModules(Pose pose, Vector3 extension, float sizeIncreaser) => OverlapBoxAtPoint(pose.position + (Vector3.up * extension.y/2), pose.rotation, extension, sizeIncreaser);

        private List<HouseModule> OverlapBoxAtPoint(Vector3 position, Quaternion rotation, Vector3 overlapExtents, float sizeIncreaser = 0f)
        {
            Collider[] overlapResult;
            List<HouseModule> modules = new List<HouseModule>();

            //Adjust overlap size
            overlapExtents += (Vector3.one * sizeIncreaser);

            //Get Colliding object
            overlapResult = Physics.OverlapBox(position, overlapExtents / 2f, rotation);

            //Get Only House Modules
            foreach (Collider col in overlapResult)
            {
                if (col.gameObject.TryGetComponent<HouseModule>(out HouseModule module))
                    modules.Add(module);
            }

            return modules;
        }
        #endregion

        #region --------------- SNAPS -------------------------------
        private Vector3 ApplySnap(ModuleData moduleToSnap, HouseModule destinationModule, Pose pose)
        {
            Quaternion snapRotation = pose.rotation;
            Vector3 destinationPos;

            //snap destination closest snap point
            Vector3 localPos = pose.position - destinationModule.transform.position;

            Vector3 closestSnappingPos;
            Vector3 snapOffset;

            //Apply Filter
            if(moduleToSnap.ModuleType == ModuleType.DOOR || moduleToSnap.ModuleType == ModuleType.WINDOW)
                closestSnappingPos = destinationModule.ModuleData.GetLocalSnappingPosition(localPos, destinationModule.transform.rotation, moduleToSnap.ModuleType);
            else
                closestSnappingPos = destinationModule.ModuleData.GetLocalSnappingPosition(localPos, destinationModule.transform.rotation);

            snapOffset = closestSnappingPos;
            closestSnappingPos += destinationModule.transform.position;

            destinationPos = closestSnappingPos;

            //Module to snap closest snap pos
            Vector3 localPos2 = destinationPos - pose.position;
            Vector3 closestSnapPos = moduleToSnap.GetLocalSnappingPosition(localPos2, snapRotation);
            Vector3 rotatedOffset = closestSnapPos;


            //closest snap pos is the global point nearest of the pose
            //destination pos is the global point nearest of the destinationpose

            Handles.color = Color.magenta;
            Handles.DrawLine(closestSnapPos + pose.position, destinationPos);

            Vector3 finalPoint = destinationPos - closestSnapPos;

            return finalPoint;
        }


        private Vector3 SnapModuleToGrid(Vector3 position)
        {
            Vector3 gridSize = EditorSnapSettings.gridSize;
            return position.Round(gridSize.x);
        }

        private void SnapRotation(float degrees)
        {
            Vector3 roundedRotation = _modulePose.rotation.eulerAngles.Round(degrees);
            _modulePose.rotation.eulerAngles = roundedRotation;
        }

        private HouseModule GetClosestModule(List<HouseModule> modules)
        {
            HouseModule closestModule = null;
            float distance = float.MaxValue;

            for (int i = 0; i<modules.Count; i++)
            {
                float dist = Vector3.Distance(modules[i].transform.position, _modulePose.position);
                if(dist < distance)
                {
                    distance = dist;
                    closestModule = modules[i];
                }
            }

            return closestModule;
        }
        #endregion

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

        #region ------------ DRAW METHODS -----------------------------
        
        private void DrawData(ModuleData selectedModuleData, Pose selectedModulePose, List<HouseModule> nearModules, List<HouseModule> collidingModules)
        {
            Vector3 selectedModuleDataCenterpos = selectedModulePose.position + (Vector3.up * selectedModuleData.Extension.y / 2);

            //Show Module Preview
            Material previewMaterial = collidingModules.Count <= 0 || _allowOverlap ? _previewMaterial_A : _previewMaterial_N;
            DrawModulePreviewAtPoint(selectedModuleData, selectedModulePose, previewMaterial);

            //Draw Module Collider
            DrawBox(selectedModuleDataCenterpos, selectedModulePose.rotation, selectedModuleData.Extension, 0f, Color.red);

            //Draw Overlap box with size multiplier
            DrawBox(selectedModuleDataCenterpos, selectedModulePose.rotation, selectedModuleData.Extension, _overlapSizeIncreaser, Color.white);

            //Draw near modules
            DrawModulesOutlines(nearModules);

            //Draw All snapping points
            DrawModuleSnapsPoints(selectedModuleData, selectedModulePose);

            foreach(HouseModule houseModule in nearModules)
                DrawModuleSnapsPoints(houseModule);
        }
        
        
        private void DrawBox(Vector3 boxPosition, Quaternion boxRotation, Vector3 boxExtents, float sizeIncreaser, Color color)
        {
            boxExtents += (Vector3.one * sizeIncreaser);

            //Sets Draw Parameters
            Handles.color = color;
            Matrix4x4 handlesMatrix = Matrix4x4.TRS(boxPosition, boxRotation, Vector3.one);
            Handles.matrix = handlesMatrix;

            //Draw Box
            Handles.DrawWireCube(Vector3.zero, boxExtents);

            Handles.matrix = Matrix4x4.identity;
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

            Handles.matrix = Matrix4x4.identity;
        }

        private void DrawModulesOutlines(List<HouseModule> modules)
        {
            if(modules.Count == 0) return;

            foreach (HouseModule module in modules)
            {
                Vector3 modulePos = module.transform.position + module.ModuleData.ColliderCenter;
                Quaternion moduleRotation = module.transform.rotation;
                Vector3 extents = module.ModuleData.Extension;
                Color color = module.ModuleData.ModuleType.ToColor();

                DrawBox(modulePos, moduleRotation, extents, 0f, color);
            }
        }

        private void DrawModuleSnapsPoints(HouseModule module)
        {
            List<SnappingPoint> snappingPoints = module.ModuleData.SnappingPoints;

            if (snappingPoints == null)
                return;

            Matrix4x4 handlesMatrix = Matrix4x4.TRS(module.transform.position, module.transform.rotation, Vector3.one);
            Handles.matrix = handlesMatrix;

            foreach (SnappingPoint snappingPoint in snappingPoints)
            {
                Color pointColor = snappingPoint.UseFilter ? snappingPoint.SnappingPointFilter.ToColor() : Color.white;
                Handles.color = pointColor;

                Handles.SphereHandleCap(-1, snappingPoint.LocalPoint, Quaternion.identity, 0.05f, EventType.Repaint);
            }

            Handles.matrix = Matrix4x4.identity;
        }

        private void DrawModuleSnapsPoints(ModuleData module, Pose pose)
        {
            List<SnappingPoint> snappingPoints = module.SnappingPoints;

            if (snappingPoints == null)
                return;

            Handles.matrix = Matrix4x4.TRS(pose.position, pose.rotation, Vector3.one);

            foreach (SnappingPoint snappingPoint in snappingPoints)
            {
                Color pointColor = snappingPoint.UseFilter ? snappingPoint.SnappingPointFilter.ToColor() : Color.white;
                Handles.color = pointColor;
                Handles.SphereHandleCap(-1, snappingPoint.LocalPoint, Quaternion.identity, 0.15f, EventType.Repaint);
            }

            Handles.matrix = Matrix4x4.identity;
        }

        private void DrawModuleSnapsPoints(ModuleData module, Pose pose, Color color)
        {
            List<SnappingPoint> snappingPoints = module.SnappingPoints;

            if (snappingPoints == null)
                return;

            Handles.matrix = Matrix4x4.identity;

            foreach (SnappingPoint snappingPoint in snappingPoints)
            {
                Handles.color = color;
                Handles.SphereHandleCap(-1, pose.position + snappingPoint.LocalPoint, Quaternion.identity, 0.25f, EventType.Repaint);
            }
        }

        #endregion
    }
}
