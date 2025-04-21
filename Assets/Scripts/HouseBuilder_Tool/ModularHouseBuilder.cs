using UnityEditor;
using UnityEngine;

using Tool.ModularHouseBuilder.SubTool;

namespace Tool.ModularHouseBuilder
{
    public class ModularHouseBuilder : EditorWindow
    {
        const string TOOL_ART_FOLDER_PATH = "Assets/Scripts/HouseBuilder_Tool/HouseBuilder_Art/";
        const string TOOL_MODULES_FOLDER_PATH = "Assets/Level/Prefabs/Building_Modules/";
        const string PREFAB_FOLDER_PATH = "Assets/Level/Prefabs";

        [MenuItem("Tools/House Builder")]
        public static void OpenHouseBuilder()
        {
            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{TOOL_ART_FOLDER_PATH}MainIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Modular House Builder", windowIcon, "Tool to create modular structures");

            //Create Window
            ModularHouseBuilder window = GetWindow<ModularHouseBuilder>();
            //Assign Fancy Graphics
            window.titleContent = titleContent;
            window.name = "House Builder";
        }

        private GUILayoutOption[] _openAssetsButtonOptions;
        private GUIContent _openAssetsButtonContent;

        private void OnEnable()
        {
            //this.BeginWindows();

            _openAssetsButtonOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
            };

            Texture icon = (Texture)AssetDatabase.LoadAssetAtPath($"{TOOL_ART_FOLDER_PATH}OpenIcon.png", typeof(Texture));
            _openAssetsButtonContent = new GUIContent("Display Assets", icon);
        }

        private void OnDisable()
        {
            
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Create New Module", GUILayout.ExpandWidth(true)))
                ModuleCreation_Window.OpenModuleCreation_Window(TOOL_ART_FOLDER_PATH, PREFAB_FOLDER_PATH, typeof(ModularHouseBuilder));

            if(GUILayout.Button("Create New Building", GUILayout.ExpandWidth(true)))
            {
                
            }

            if (GUILayout.Button(_openAssetsButtonContent, _openAssetsButtonOptions))
                ModulesExplorer_Window.OpenExplorer_Window(TOOL_ART_FOLDER_PATH, TOOL_MODULES_FOLDER_PATH, typeof(ModularHouseBuilder));
        }
    }
}