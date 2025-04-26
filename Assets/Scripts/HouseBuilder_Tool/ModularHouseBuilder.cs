using UnityEditor;
using UnityEngine;

using Tool.ModularHouseBuilder.SubTool;

namespace Tool.ModularHouseBuilder
{
    public class ModularHouseBuilder : EditorWindow
    {
        const string TOOL_ART_FOLDER_PATH = "Assets/Scripts/HouseBuilder_Tool/HouseBuilder_Art/";
        const string PREFAB_FOLDER_PATH = "Assets/Level/Prefabs";

        [MenuItem("Tools/House Builder")]
        public static void OpenHouseBuilder()
        {
            if (!EditorPrefs.HasKey("ModularHouseBuilder_PREFAB_FOLDER"))
                EditorPrefs.SetString("ModularHouseBuilder_PREFAB_FOLDER", PREFAB_FOLDER_PATH);
            if(!EditorPrefs.HasKey("ModularHouseBuilder_ART_FOLDER"))
                EditorPrefs.SetString("ModularHouseBuilder_ART_FOLDER", TOOL_ART_FOLDER_PATH);

            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{TOOL_ART_FOLDER_PATH}MainIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Modular House Builder", windowIcon, "Tool to create modular structures");

            //Create Window
            ModularHouseBuilder window = GetWindow<ModularHouseBuilder>();
            //Assign Fancy Graphics
            window.titleContent = titleContent;
            window.name = "House Builder";
        }

        private GUIContent _buildingCreationButtonContent;
        private GUILayoutOption[] _buildingCreationButtonOptions;

        private GUIContent _moduleCreationButtonContent;
        private GUILayoutOption[] _moduleCreationButtonOptions;

        private GUIContent _moduleButtonContent;
        private GUILayoutOption[] _modulesButtonOptions;

        private ModulesExplorer_Window _explorerWindow;

        private void OnEnable()
        {
            //Create Module Settings
            Texture createModuleTexture = (Texture)AssetDatabase.LoadAssetAtPath($"{TOOL_ART_FOLDER_PATH}AddModuleIcon.png", typeof(Texture));
            _moduleCreationButtonContent = new GUIContent(" Create New Module", createModuleTexture);

            _moduleCreationButtonOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true),
            };

            //Create Building Settings
            Texture createBuildingTexture = (Texture)AssetDatabase.LoadAssetAtPath($"{TOOL_ART_FOLDER_PATH}NewBuildingIcon.png", typeof(Texture));
            _buildingCreationButtonContent = new GUIContent(" Create New Building", createBuildingTexture);

            _buildingCreationButtonOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true),
            };


            //Open Module Settings
            Texture openFolderTexture = (Texture)AssetDatabase.LoadAssetAtPath($"{TOOL_ART_FOLDER_PATH}SearchIcon.png", typeof(Texture));
            _moduleButtonContent = new GUIContent(" Display Modules", openFolderTexture);

            _modulesButtonOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true),
            };


        }

        private void OnDisable()
        {
            
        }

        private void OnGUI()
        {
            //Module Creation Button
            if(GUILayout.Button(_moduleCreationButtonContent, _moduleCreationButtonOptions))
                ModuleCreation_Window.OpenWindow(typeof(ModularHouseBuilder));

            GUILayout.Space(5f);

            //Building Creation Button
            if (GUILayout.Button(_buildingCreationButtonContent, _buildingCreationButtonOptions))
                BuildingCreation_Window.OpenWindow(typeof(ModularHouseBuilder));

            GUILayout.Space(5f);

            //Module Explorer
            if (GUILayout.Button(_moduleButtonContent, _modulesButtonOptions))
                if(_explorerWindow == null)
                    _explorerWindow = new ModulesExplorer_Window(typeof(ModularHouseBuilder));
        }
    }
}