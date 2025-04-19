using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tool.ModularHouseBuilder.SubTool
{
    public class ModulesExplorer_Window : EditorWindow
    {
        private static string s_artAssetsPath;
        private static string s_assetsPath;

        private ModuleData _selectedModule;

        public static void OpenExplorer_Window(string artAssetPath, string assetPath, Type dockNextTo)
        {
            s_artAssetsPath = artAssetPath;
            s_assetsPath = assetPath;

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
            public GUIContent GUIContent;
        }
        private ExplorerTabData[] _explorerTabs;

        //Search bar
        private string _searchInput;
        private GUILayoutOption[] _searchGUIOptions;

        //Scroll view
        private Vector2 _scrollPos;
        private GUILayoutOption[] _scrollBarGUIOptions;

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

                //Assisgn tab name
                _explorerTabs[i].Name = type.ToAssetName();

                //Asset name style
                GUIStyle titleStyle = new GUIStyle()
                {
                    fontSize = 30,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                titleStyle.normal.textColor = Color.white;
                //Assign Tab Style
                _explorerTabs[i].GUIStyle = titleStyle;
                

                //Get Modules Data
                string path = $"{s_assetsPath}{type.ToFolderName()}/";
                string[] assetsPath = Directory.GetFiles(path, "*.asset");
                List<ModuleData> modules = new List<ModuleData>();

                //Load All assets
                foreach (string assetPath in assetsPath)
                    modules.Add(AssetDatabase.LoadAssetAtPath<ModuleData>(assetPath));

                //Add Module Data
                _explorerTabs[i].ModulesData = modules;
            }

            //Create tab options
            _toolbarGUIOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(false),
            };

            //Set up Search Bar
            _searchGUIOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
            };

            //Setup ScrollView Scope
            _scrollPos = Vector2.zero;
            _scrollBarGUIOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true),
            };

            Repaint();
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

            DrawModules(tab.ModulesData);

            //Input Handling
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

        private void DrawModules(List<ModuleData> moduleDatas)
        {
            //Search Bar
            GUILayout.Label("Search", GUILayout.ExpandWidth(true));
            _searchInput = GUILayout.TextField(_searchInput, _searchGUIOptions);

            Rect windowRect = position;
            Texture texture = GetTexture(moduleDatas[0]);
            Vector2 textureSize = new Vector2(texture.width, texture.height);
            int texturePerWidth = (int)(windowRect.width / texture.width);
            if (texturePerWidth <= 0)
                texturePerWidth = 1;

            //Draw All Moule in the current selected tab
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, _scrollBarGUIOptions);
            {
                for(int i = 0; i < moduleDatas.Count; i+= texturePerWidth)
                {
                    int endLineIndex = i + texturePerWidth;
                    endLineIndex = Mathf.Clamp(endLineIndex, 0, moduleDatas.Count);

                    List<ModuleData> modulesToShow = new List<ModuleData>(texturePerWidth);
                    modulesToShow.AddRange( moduleDatas.GetRange(i, endLineIndex) );

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        foreach(ModuleData moduleData in modulesToShow)
                        {
                            GUILayout.Button(moduleData.Preview);
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }

        private Texture GetTexture(ModuleData moduleData)
        {
            GameObject gameObject = moduleData.Prefab;
            Texture preview = moduleData.Preview;

            if(preview == null)
            {
                preview = AssetPreview.GetAssetPreview(gameObject);
                moduleData.Preview = preview;
            }

            return preview;
        }
    }
}
