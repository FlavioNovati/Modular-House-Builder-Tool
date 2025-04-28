using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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

        private void OnEnable()
        {
            if (_moduleExplorer == null)
            {
                //Open Module Displayer 
                _moduleExplorer = ModulesExplorer_Window.OpenWindow();
            }
            _moduleExplorer.ShowModuleInfo(false);
            _moduleExplorer.OnModuleSelected += UpdateSelectedModule;
            _moduleExplorer.OnWindowDestroyed += Close;

            //Get Building
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                Debug.LogError("ERROR - Not in prefab mode");
                this.Close();
                _moduleExplorer?.Close();
                return;
            }
            _building = prefabStage.prefabContentsRoot.GetComponent<Building>();
            _buildingData = _building.BuildingData;

            //Load materials
            LoadPreviewMaterial();

            _modulePose = new Pose(Vector3.zero, Quaternion.identity);

            SceneView.duringSceneGui += OnSceneGUI;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnDisable()
        {
            _moduleExplorer.OnModuleSelected -= UpdateSelectedModule;
            SceneView.duringSceneGui -= OnSceneGUI;
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }


        private void LoadPreviewMaterial()
        {
            string materialAssetPath = EditorPrefs.GetString("ModularHouseBuilder_ART_FOLDER");
            string material_AcceptedName = "PreviewMaterial_A.mat";
            string material_NegatedName = "PreviewMaterial_N.mat";

            _previewMaterial_A = AssetDatabase.LoadAssetAtPath($"{materialAssetPath}{material_AcceptedName}", typeof(Material)) as Material;
            _previewMaterial_N = AssetDatabase.LoadAssetAtPath($"{materialAssetPath}{material_NegatedName}", typeof(Material)) as Material;
        }

        private void UpdateSelectedModule(ModuleData moduleData) => _selectedModuleData = moduleData;






        //----------------------------------TOOL SCENE VIEW------------------------------------------------------------
        private void OnSceneGUI(SceneView sceneView)
        {
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
                //Update Module Position
                _modulePose.position = ray.GetPoint(enter);

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

                //Show Preview
                DrawModulePreviewAtPoint(_selectedModuleData, _modulePose);

                //Repaint Scene View
                SceneView.RepaintAll();
            }


            if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                InstanciateModule(_selectedModuleData);
                Event.current.Use();
            }

            if(Input.GetKey(KeyCode.Escape) && _selectedModuleData != null)
            {
                _selectedModuleData = null;
            }

            //Overlap Box
            //Show Near Modules
        }



        private void DrawModulePreviewAtPoint(ModuleData moduleToShow, Pose modulePose)
        {
            HouseBuilderModule module = moduleToShow.Module;

            MeshFilter[] meshFilters = module.GetComponentsInChildren<MeshFilter>();
            Matrix4x4 positionToWorldMatrix = Matrix4x4.TRS(modulePose.position, modulePose.rotation, Vector3.one);

            foreach (MeshFilter filter in meshFilters)
            {
                Matrix4x4 childLocalMatrix = filter.transform.localToWorldMatrix;
                Matrix4x4 childToWorldMatrix = positionToWorldMatrix * childLocalMatrix;

                Material previewMaterial = _previewMaterial_A;
                previewMaterial.SetPass(0);

                Graphics.DrawMeshNow(filter.sharedMesh, childToWorldMatrix);
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


        private void InstanciateModule(ModuleData moduleData)
        {
            HouseBuilderModule module = Instantiate<HouseBuilderModule>(moduleData.Module, _building.transform);
            module.transform.position = _modulePose.position;
            module.transform.rotation = _modulePose.rotation;

            _buildingData.AddModule(moduleData);

            Undo.RegisterCreatedObjectUndo(module, "Instanciated "+moduleData.ModuleName);
            Undo.RecordObject(module, "");
            Undo.RecordObject(_building, "Edited Building");
            Undo.FlushUndoRecordObjects();
        }

        private void OnUndoRedoPerformed()
        {

        }

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
    }
}
