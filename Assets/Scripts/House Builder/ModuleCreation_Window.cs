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

        public static void OpenModuleCreation_Window() => GetWindow<ModuleCreation_Window>("Module Creation", true);
        
        private string _moduleName = null;
        private string _modulePath = null;
        private Module_Type _moduleType = Module_Type.WALL;

        private void OnEnable()
        {
            //Create Main Folder for assets
            CreateModuleFolder(PREFAB_FOLDER_PATH, PREFAB_FOLDER_NAME);

            //Create Module Type Folders
            int enumEntries = Enum.GetValues(typeof(Module_Type)).Length;
            string parentPath = $"{PREFAB_FOLDER_PATH}/{PREFAB_FOLDER_NAME}";
            //Add First entry Folder
            CreateModuleFolder(parentPath, ((Module_Type) 0).ToFolderName());
            //Add All the other folders
            for (int i = 1; i < Mathf.Pow(2, enumEntries); i*=2)
            {
                Module_Type entry = (Module_Type)i;
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
            GUILayout.Label("Asset Type");
            _moduleType = (Module_Type)EditorGUILayout.EnumPopup(_moduleType, GUILayout.ExpandWidth(true));
            GUILayout.Label("Asset Name");
            _moduleName = GUILayout.TextField(_moduleName, GUILayout.ExpandWidth(true));
            if (!string.IsNullOrEmpty(_moduleName))
                _moduleName = _moduleName.Replace(' ', '_');

            //Create Module Button (only if module have a name)
            if (!string.IsNullOrEmpty(_moduleName))
            {
                if (GUILayout.Button($"Create {PREFAB_PREFIX}{_moduleType.ToAssetName()}_{_moduleName}", GUILayout.ExpandWidth(true)))
                {
                    //Create Module
                    string assetName = $"{_moduleType.ToAssetName()}_{_moduleName}";
                    string assetPath = $"{PREFAB_FOLDER_PATH}/{PREFAB_FOLDER_NAME}/{_moduleType.ToFolderName()}";
                    CreateModule(assetName, assetPath);
                    //Lose Focus
                    GUI.FocusControl(null);

                    //Enter Prefab Mode
                    PrefabStageUtility.OpenPrefab(_modulePath);
                }
            }

            //Pings
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Ping Folder", GUILayout.ExpandWidth(true)))
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(PREFAB_FOLDER_PATH + "/" + PREFAB_FOLDER_NAME, typeof(object)));

                if (!string.IsNullOrEmpty(_modulePath))
                {
                    if (GUILayout.Button($"Ping {_moduleName}", GUILayout.ExpandWidth(true)))
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(_modulePath, typeof(object)));
                }
            }

            //Lose Focus
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                GUI.FocusControl(null);
                Event.current.Use();
                Repaint();
            }
        }

        private void CreateModule(string name, string path)
        {
            //Get Module path
            string modulePath = $"{path}/{PREFAB_PREFIX}{name}.prefab";
            modulePath = AssetDatabase.GenerateUniqueAssetPath(modulePath);
            
            //Create Prefab
            GameObject prefab = new GameObject(modulePath);
            PrefabUtility.SaveAsPrefabAsset(prefab, modulePath);
            //Destroy instanciated game object in scene
            DestroyImmediate(prefab);

            //Update Module Path
            _modulePath = modulePath;
        }
    }
}
