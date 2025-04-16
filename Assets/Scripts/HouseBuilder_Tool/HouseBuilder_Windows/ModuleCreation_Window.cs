using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tool.ModularHouseBuilder.SubTool
{
    public class ModuleCreation_Window : EditorWindow
    {
        const string PREFAB_FOLDER_PATH = "Assets/Level/Prefab";
        const string PREFAB_FOLDER_NAME = "Building_Modules";
        const string PREFAB_PREFIX = "Module_";
        const string SCRIPTABLE_PREFIX = "ModuleData_";

        private string _moduleName = null;
        private string _modulePath = null;
        private ModuleType _moduleType = ModuleType.WALL;

        private GameObject _modelGameObject = null;
        private MeshFilter _meshFilter = null;
        private MeshRenderer _meshRenderer = null;

        private Vector3 _meshRotation = Vector3.zero;
        private Vector3 _meshScale = Vector3.one;
        private bool _offsetByHalfHeight = false;

        public static void OpenModuleCreation_Window(string artAssetPath, Type dockNextTo)
        {
            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{artAssetPath}ModuleIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Module Creator", windowIcon, "Tool to create modules");

            ModuleCreation_Window window = GetWindow<ModuleCreation_Window>("", true, dockNextTo);

            //Add Graphics
            window.titleContent = titleContent;
            window.name = "Module Builder";
        }

        private void OnEnable()
        {
            //Create Main Folder for assets
            CreateModuleFolder(PREFAB_FOLDER_PATH, PREFAB_FOLDER_NAME);

            //Create Module Type Folders
            int enumEntries = Enum.GetValues(typeof(ModuleType)).Length;
            string parentPath = $"{PREFAB_FOLDER_PATH}/{PREFAB_FOLDER_NAME}";
            
            for (int i = 0; i < enumEntries; i++)
            {
                ModuleType entry = ModuleTypeUtils.ModuleTypeFromInt(i);
                CreateModuleFolder(parentPath, entry.ToFolderName());
            }
        }

        private void CreateModuleFolder(string path, string name)
        {
            if (!AssetDatabase.IsValidFolder($"{path}/{name}"))
                AssetDatabase.CreateFolder(path, name);
        }

        private void OnDisable()
        {

        }

        private void OnGUI()
        {
            //Module Settings
            GUILayout.Space(5f);
            GUILayout.Label("Asset Type");
            using (new GUILayout.HorizontalScope())
            {
                //Update Module Type
                _moduleType = (ModuleType)EditorGUILayout.EnumPopup(_moduleType, GUILayout.ExpandWidth(true));
                //Module Type Texture
                GUIContent enumContent = new GUIContent(_moduleType.ToTexture(), _moduleType.ToDescription());
                GUILayout.Label(enumContent);
                //Add space
                GUILayout.Space(15f);
                //Allow Pinging Type folder
                if (GUILayout.Button($"Ping Folder", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(80f)))
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(GetAssetFolderPath(_moduleType), typeof(object)));
            }

            //Name of the Module
            GUILayout.Space(5f);
            GUILayout.Label("Module Name");
            _moduleName = GUILayout.TextField(_moduleName, GUILayout.ExpandWidth(true));
            if (!string.IsNullOrEmpty(_moduleName))
                _moduleName = _moduleName.Replace(' ', '_');

            //Model of the Module
            GUILayout.Space(5f);
            GUILayout.Label("Module 3D Model");
            if(GUILayout.Button("Select Model", GUILayout.ExpandWidth(true)))
                EditorGUIUtility.ShowObjectPicker<GameObject>(_modelGameObject, false, "t: Model", -1);
            //Draw Preview if possible
            if (_modelGameObject != null)
                GUILayout.Box(AssetPreview.GetAssetPreview(_modelGameObject), GUILayout.ExpandWidth(true));
            //Mesh Rotation
            _meshRotation = EditorGUILayout.Vector3Field("Mesh Rotation", _meshRotation, GUILayout.ExpandWidth(true));
            _meshScale = EditorGUILayout.Vector3Field("Mesh Scale", _meshScale, GUILayout.ExpandWidth(true));
            _offsetByHalfHeight = GUILayout.Toggle(_offsetByHalfHeight, "Offset by half height", GUILayout.ExpandWidth(true));


            GUILayout.Space(5f);
            //Create Module Button (only if module have a Name and a Mesh)
            if (!string.IsNullOrEmpty(_moduleName) && _modelGameObject != null)
            {
                if (GUILayout.Button($"Create {PREFAB_PREFIX}{_moduleType.ToAssetName()}_{_moduleName}", GUILayout.ExpandWidth(true)))
                {
                    //Create Module
                    string moduleName = $"{_moduleType.ToAssetName()}_{_moduleName}";
                    string moduleFolderPath = GetAssetFolderPath(_moduleType);
                    CreateModule(moduleName, moduleFolderPath);


                    //Lose Focus
                    GUI.FocusControl(null);
                    //Enter Prefab Mode
                    PrefabStageUtility.OpenPrefab(_modulePath);
                }
            }

            //Pings
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Ping Modules Folder", GUILayout.ExpandWidth(true)))
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(PREFAB_FOLDER_PATH + "/" + PREFAB_FOLDER_NAME, typeof(object)));

                if (!string.IsNullOrEmpty(_modulePath))
                {
                    if (GUILayout.Button($"Ping {_moduleName}", GUILayout.ExpandWidth(true)))
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(_modulePath, typeof(object)));
                }
            }

            //Event Handling
            Event currentEvent = Event.current;
            EventType currentEventType = currentEvent.type;

            //Object picked
            if(currentEventType == EventType.ExecuteCommand)
            {
                if(currentEvent.commandName == "ObjectSelectorUpdated")
                {
                    _modelGameObject = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                    if(_modelGameObject != null)
                    {
                        _meshFilter = _modelGameObject.GetComponent<MeshFilter>();
                        _meshRenderer = _modelGameObject.GetComponent<MeshRenderer>();
                        Repaint();
                    }
                }
            }

            //Lose Focus
            if (currentEventType == EventType.MouseDown && currentEvent.button == 0)
            {
                GUI.FocusControl(null);
                currentEvent.Use();
                Repaint();
            }
        }

        private void CreateModule(string name, string folderPath)
        {
            //Get Module path
            string modulePath = $"{folderPath}/{PREFAB_PREFIX}{name}.prefab";
            modulePath = AssetDatabase.GenerateUniqueAssetPath(modulePath);
            
            //Parent
            // -MESH- (Mesh Holder)
            //  Mesh (Actual Mesh)

            //Create Parent
            GameObject prefab = new GameObject(name, typeof(HouseBuilderModule), typeof(BoxCollider));

            //Create Mesh Holder Child
            GameObject meshHolder = new GameObject("-MESH-");
            meshHolder.transform.parent = prefab.transform;
            
            //Create Mesh Game Object
            GameObject meshGameObject = new GameObject("Module_Mesh", typeof(MeshFilter), typeof(MeshRenderer));
            meshGameObject.transform.parent = meshHolder.transform;
            meshGameObject.transform.eulerAngles = _meshRotation;
            meshGameObject.transform.localScale = _meshScale;
            meshGameObject.GetComponent<MeshFilter>().mesh = _meshFilter.sharedMesh;
            meshGameObject.GetComponent<MeshRenderer>().materials = _meshRenderer.sharedMaterials;
            
            //Move Mesh Game Object acccording to rotation, scale and bounds
            Vector3 meshOffset = _meshFilter.sharedMesh.bounds.center;
            meshOffset.x *= _meshScale.x;
            meshOffset.y *= _meshScale.y;
            meshOffset.z *= _meshScale.z;
            //Rotate Offset
            meshOffset = Quaternion.Euler(_meshRotation) * meshOffset;
            //Apply Offset
            meshGameObject.transform.position -= meshOffset;
            
            //Get Collider Size
            Vector3 colliderSize = _meshFilter.sharedMesh.bounds.extents * 2f;
            colliderSize.x *= _meshScale.x;
            colliderSize.z *= _meshScale.z;
            colliderSize.y *= _meshScale.y;
            colliderSize = Quaternion.Euler(_meshRotation) * colliderSize;

            colliderSize.x = Mathf.Abs(colliderSize.x);
            colliderSize.y = Mathf.Abs(colliderSize.y);
            colliderSize.z = Mathf.Abs(colliderSize.z);

            //Move mesh by offset
            Vector3 offset = Vector3.zero;
            if (_offsetByHalfHeight)
                offset.y += colliderSize.y / 2f;

            meshGameObject.transform.position += offset;

            //Set box collider
            BoxCollider boxCollider = prefab.GetComponent<BoxCollider>();
            boxCollider.size = colliderSize;
            boxCollider.center = offset;


            //Save Prefab
            PrefabUtility.SaveAsPrefabAsset(prefab, modulePath);

            //Create Scriptable
            CreateAndLinkData(name, folderPath, modulePath, colliderSize, offset);

            //Destroy instanciated game object in scene
            DestroyImmediate(prefab);

            //Update Module Path
            _modulePath = modulePath;
        }

        private void CreateAndLinkData(string name, string saveFolderPath, string prefabAssetPath, Vector3 extension, Vector3 centerOffset)
        {
            //Instanciate Scriptable
            ModuleData module_Data = ScriptableObject.CreateInstance<ModuleData>();
            module_Data.Prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabAssetPath, typeof(GameObject));
            //Link Data
            module_Data.Prefab.GetComponent<HouseBuilderModule>().ModuleData = module_Data;

            //Get Path
            string moduleDataPath = $"{saveFolderPath}/{SCRIPTABLE_PREFIX}{name}.asset";
            moduleDataPath = AssetDatabase.GenerateUniqueAssetPath(moduleDataPath);

            //Create Scriptable Asset
            module_Data.SetPreview(null);
            module_Data.ModuleType = _moduleType;
            module_Data.Extension = extension;
            module_Data.CenterOffset = centerOffset;
            module_Data.Rotation = _meshRotation;

            //Create Asset
            AssetDatabase.CreateAsset(module_Data, moduleDataPath);
            //Update Prefab
            PrefabUtility.SavePrefabAsset(module_Data.Prefab);
        }

        private string GetAssetFolderPath(ModuleType moduleType) => $"{PREFAB_FOLDER_PATH}/{PREFAB_FOLDER_NAME}/{moduleType.ToFolderName()}";
    }
}
