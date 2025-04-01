using System;
using UnityEditor;
using UnityEngine;

namespace Tool.ModularHouseBuilder.SubTool
{
    public class ModulesExplorer_Window : EditorWindow
    {
        private int _selectedTab;

        GUIContent[] _toolbarContent;
        GUILayoutOption[] _toolbarOptions;

        [MenuItem("Tools/Module Editor")]
        public static void OpenHouseBuilder()
        {
            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Scripts/HouseBuilder_Tool/HouseBuilder_Art/ModuleEditorIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Module Editor", windowIcon, "Tool to edit modules");

            //Create Window
            ModulesExplorer_Window window = GetWindow<ModulesExplorer_Window>();
            //Assign Fancy Graphics
            window.titleContent = titleContent;
            window.name = "Module Editor";
        }

        private void OnEnable()
        {
            _selectedTab = 0;

            //Create tabs elements
            int tabsCount = Enum.GetValues(typeof(ModuleType)).Length;
            _toolbarContent = new GUIContent[tabsCount];

            //Get all content
            for (int i = 0; i < tabsCount; i++)
            {
                int typeValue = (int)Mathf.Pow(2, i);

                if (i == 0)
                    typeValue = 0;

                string tabDescription = ((ModuleType)typeValue).ToDescription();
                Texture tabTexture = ((ModuleType)typeValue).ToTexture();

                _toolbarContent[i] = new GUIContent(tabTexture, tabDescription);
            }

            //Create options
            _toolbarOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(false),
            };
        }

        private void OnDisable()
        {
            
        }

        private void OnGUI()
        {
            _selectedTab = GUILayout.Toolbar(_selectedTab, _toolbarContent, _toolbarOptions);
            //Show all modules of the selected tab
        }
    }
}
