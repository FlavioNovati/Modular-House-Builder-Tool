using UnityEditor;
using UnityEngine;

using Tool.ModularHouseBuilder.SubTool;

namespace Tool.ModularHouseBuilder
{
    public class ModularHouseBuilder : EditorWindow
    {
        const string TOOL_FOLDER_PATH = "Assets/Scripts/HouseBuilder_Tool/";

        [MenuItem("Tools/House Builder")]
        public static void OpenHouseBuilder()
        {
            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{TOOL_FOLDER_PATH}HouseBuilder_Art/MainIcon.png", typeof(Texture));
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

            Texture icon = (Texture)AssetDatabase.LoadAssetAtPath($"{TOOL_FOLDER_PATH}HouseBuilder_Art/OpenIcon.png", typeof(Texture));
            _openAssetsButtonContent = new GUIContent("Display Assets", icon);
        }

        private void OnDisable()
        {
            //this.EndWindows();
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Create New Module", GUILayout.ExpandWidth(true)))
                ModuleCreation_Window.OpenModuleCreation_Window(typeof(ModularHouseBuilder));

            if(GUILayout.Button("Create New Building", GUILayout.ExpandWidth(true)))
            {
                
            }

            if (GUILayout.Button(_openAssetsButtonContent, _openAssetsButtonOptions))
                ModulesExplorer_Window.OpenExplorer_Window(TOOL_FOLDER_PATH, typeof(ModularHouseBuilder));
        }
    }
}