using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Tool.ModularHouseBuilder.SubTool
{
    public class ModulesExplorer_Window : EditorWindow
    {
        private static string s_assetPath;
        
        public static void OpenExplorer_Window(string assetPath, Type dockNextTo)
        {
            s_assetPath = assetPath;
            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{s_assetPath}HouseBuilder_Art/ModuleEditorIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Module Explorer", windowIcon, "Tool to edit modules");

            //Create Window
            ModulesExplorer_Window window = GetWindow<ModulesExplorer_Window>("", true, dockNextTo);
            //Assign Fancy Graphics
            window.titleContent = titleContent;
            window.name = "Module Explorer";
        }


        //Tabs parameters
        private int _selectedTab;
        GUIContent[] _toolbarGUIContent;
        GUILayoutOption[] _toolbarGUIOptions;
        struct ExplorerTabData
        {
            public string Name;
            public List<ModuleData> ModulesData;
        }
        private ExplorerTabData[] _explorerTabs;

        //Search bar
        private string _searchInput;
        private GUIContent _searchGUIContent;
        private GUILayoutOption[] _searchGUIOptions;

        private void OnEnable()
        {
            //SET UP TABS
            _selectedTab = 0;

            //Create tabs elements
            int tabsCount = Enum.GetValues(typeof(ModuleType)).Length;
            _toolbarGUIContent = new GUIContent[tabsCount];
            _explorerTabs = new ExplorerTabData[tabsCount];

            //Get all content
            for (int i = 0; i < tabsCount; i++)
            {
                ModuleType type = ModuleTypeUtils.ModuleTypeFromInt(i);

                string tabDescription = type.ToDescription();
                Texture tabTexture = type.ToTexture();

                _toolbarGUIContent[i] = new GUIContent(tabTexture, tabDescription);
                _explorerTabs[i].Name = type.ToAssetName();
            }

            //Create tab options
            _toolbarGUIOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(false),
            };

            //SET UP SEARCH BAR
            Texture searchIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{s_assetPath}HouseBuilder_Art/SearchIcon.png", typeof(Texture));
            _searchGUIContent = new GUIContent(searchIcon);
            _searchGUIOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
            };

            GetAssets();
        }

        private void OnDisable()
        {
            
        }

        private void OnGUI()
        {
            _selectedTab = GUILayout.Toolbar(_selectedTab, _toolbarGUIContent, _toolbarGUIOptions);
            Rect windowRect = position;

            //Show all modules of the selected tab

            ExplorerTabData tab = _explorerTabs[_selectedTab];
            ModuleType moduleType = ModuleTypeUtils.ModuleTypeFromInt(_selectedTab);

            //Show Asset Name
            GUIStyle titleStyle = new GUIStyle()
            {
                fontSize = 30,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(5, 0, 0, 0),
            };
            titleStyle.normal.textColor = Color.white;
            

            GUILayout.Label(tab.Name, titleStyle);
            
            //SEARCH BAR
            //Draw Seach bar icon
            using(new GUILayout.HorizontalScope())
            {
                GUILayout.Label(_searchGUIContent);
                _searchInput = GUILayout.TextField(_searchInput, _searchGUIOptions);
            }

            //Show assets
            //TODO: CONTINUE




            //INPUT HANDLING
            Event currentEvent = Event.current;
            EventType currentEventType = Event.current.type;

            //Lose Focus
            if (currentEventType == EventType.MouseDown && currentEvent.button == 0)
            {
                GUI.FocusControl(null);
                currentEvent.Use();
                Repaint();
            }
        }

        private void GetAssets()
        {
            //ist<ModuleData> AllModulesData;

        }
    }
}
