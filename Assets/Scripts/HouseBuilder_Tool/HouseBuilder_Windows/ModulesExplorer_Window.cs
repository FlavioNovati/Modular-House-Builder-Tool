using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.MessageBox;

namespace Tool.ModularHouseBuilder.SubTool
{
    public class ModulesExplorer_Window : EditorWindow
    {
        private static string s_artAssetsPath;
        
        public static void OpenExplorer_Window(string assetPath, Type dockNextTo)
        {
            s_artAssetsPath = assetPath;
            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{s_artAssetsPath}SearchIcon.png", typeof(Texture));
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
            public GUIStyle GUIStyle;
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

                //Get all information about tab
                string tabDescription = type.ToDescription();
                Texture tabTexture = type.ToTexture();

                _toolbarGUIContent[i] = new GUIContent(tabTexture, tabDescription);
                _explorerTabs[i].Name = type.ToAssetName();

                //Asset name style
                GUIStyle titleStyle = new GUIStyle()
                {
                    fontSize = 30,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                titleStyle.normal.textColor = Color.white;
                _explorerTabs[i].GUIStyle = titleStyle;

                //Get Modules Data
                string path = $"{s_artAssetsPath}/{type.ToFolderName()}";
                List<UnityEngine.Object> datas = AssetDatabase.LoadAllAssetsAtPath(path).ToList();

                //Filter assets
                //IEnumerable<ModuleData> modulesData = datas.Where(moduleData => moduleData.GetType() == typeof(ScriptableObject));

                //Assign assets
                
            }

            //Create tab options
            _toolbarGUIOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(false),
            };

            //SET UP SEARCH BAR
            Texture searchIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{s_artAssetsPath}SearchIcon.png", typeof(Texture));
            _searchGUIContent = new GUIContent("", searchIcon);
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
            //Draw Label For Tab Name
            GUILayout.Label(tab.Name, tab.GUIStyle);

            //SEARCH BAR
            //Draw Seach bar icon
            GUILayout.Label(_searchGUIContent);
            _searchInput = GUILayout.TextField(_searchInput, _searchGUIOptions);

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
            //Update Visual
        }
    }
}
