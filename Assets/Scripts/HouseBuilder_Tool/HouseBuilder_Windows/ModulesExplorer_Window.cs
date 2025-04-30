using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tool.ModularHouseBuilder.SubTool
{
    public class ModulesExplorer_Window : EditorWindow
    {
        public delegate void WindowCallback(ModuleData moduleData);
        public event WindowCallback OnModuleSelected;

        public delegate void WindowActionCallback();
        public event WindowActionCallback OnWindowClosed;
        public event WindowActionCallback OnWindowDestroyed;

        public static ModulesExplorer_Window OpenWindow() => OpenWindow(null);
        public static ModulesExplorer_Window OpenWindow(Type dockType)
        {
            string artAssetPath = EditorPrefs.GetString("ModularHouseBuilder_ART_FOLDER");

            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{artAssetPath}SearchIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Module Explorer", windowIcon, "Tool to edit modules");

            ModulesExplorer_Window window;

            if(dockType != null)
                window = GetWindow<ModulesExplorer_Window>(dockType);
            else
                window = GetWindow<ModulesExplorer_Window>();

            window.titleContent = titleContent;
            window.name = "Module Explorer";

            return window;
        }

        private ModuleData SelectedModuleProperty
        {
            get => _selectedModule;
            set
            {
                _selectedModule = value;
                OnModuleSelected?.Invoke(value);
            }
        }

        private ModuleData _selectedModule;


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

        private GUIStyle _moduleNameStyle;
        private string _assetsPath;
        private bool _showModuleInfo = true;

        private void OnEnable()
        {
            _assetsPath = EditorPrefs.GetString("ModularHouseBuilder_MODULES_FOLDER");

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
                string path = $"{_assetsPath}{type.ToFolderName()}/";
                string[] assetsPath = Directory.GetFiles(path, "*.asset");
                List<ModuleData> modules = new List<ModuleData>();

                //Load All assets
                foreach (string assetPath in assetsPath)
                    modules.Add(AssetDatabase.LoadAssetAtPath<ModuleData>(assetPath));
                modules.Reverse();

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

            _moduleNameStyle = new GUIStyle()
            {
                fontSize = 30,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _moduleNameStyle.normal.textColor = Color.white;     

            Repaint();
        }

        private void OnDisable() => OnWindowClosed?.Invoke();
        private void OnDestroy()
        {
            OnWindowClosed?.Invoke();
            OnWindowDestroyed?.Invoke();
        }

        private void OnGUI()
        {
            int currentTab = _selectedTab;
            _selectedTab = GUILayout.Toolbar(_selectedTab, _toolbarGUIContent, _toolbarGUIOptions);
            
            //Check if user changed tab
            if(currentTab != _selectedTab)
                SelectedModuleProperty = null;

            //Draw Tab Or Selected Module Property
            if (_showModuleInfo && SelectedModuleProperty != null)
            {
                DrawModule(SelectedModuleProperty);
            }
            else
            {
                ExplorerTabData tab = _explorerTabs[_selectedTab];

                //Update all previews
                if (GUILayout.Button("Update All Preview", GUILayout.ExpandWidth(true)))
                    UpdateAllPreviews(tab.ModulesData);

                DrawResearchBar();
                DrawTab(tab);
            }


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

        private void DrawResearchBar()
        {
            //Search Bar
            GUILayout.Space(15f);
            GUILayout.Label("Search", GUILayout.ExpandWidth(true));
            _searchInput = GUILayout.TextField(_searchInput, _searchGUIOptions);
            GUILayout.Space(15f);
        }

        private void DrawTab(ExplorerTabData tab)
        {
            //Draw Label For Tab Name
            GUILayout.Label(tab.Name, tab.GUIStyle);
            DrawModules(tab.ModulesData);
        }

        private void DrawModule(ModuleData moduleData)
        {
            GUILayout.Label(moduleData.ModuleName, _moduleNameStyle);
            GUILayout.Space(15f);
            GUILayout.Label(moduleData.Preview, new GUIStyle() { alignment = TextAnchor.MiddleCenter } );
            GUILayout.Space(15f);
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("OpenWindow Module"))
                {
                    string prefabPath = AssetDatabase.GetAssetPath(moduleData.Module.gameObject);
                    PrefabStageUtility.OpenPrefab(prefabPath);

                    //Focus Insoector tab
                    Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
                    // Focus on the Inspector tab
                    EditorWindow.GetWindow(inspectorType).Focus();

                }
                if (GUILayout.Button("Back"))
                    SelectedModuleProperty = null;
            }

        }

        private void DrawModules(List<ModuleData> moduleDatas)
        {
            Rect windowRect = position;
            Vector2 textureSize = new Vector2(128, 128);

            //Get how many modules can be drawn on a row
            int modulesPerRow = Mathf.FloorToInt(windowRect.width / textureSize.x);
            if (modulesPerRow <= 0)
                modulesPerRow = 1;

            List<ModuleData> modulesToShow = new List<ModuleData>();

            //Apply search Filter
            if (!string.IsNullOrEmpty(_searchInput))
                modulesToShow.AddRange(moduleDatas.Where(data => data.ModuleName.ToLower().Contains(_searchInput.ToLower())));
            else
                modulesToShow.AddRange(moduleDatas);
            
            //Draw All Moule in the current selected tab
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, _scrollBarGUIOptions);
            {
                int currentRow = 0;
                int currentIndex = 0;
                
                do
                {
                    using (new GUILayout.VerticalScope())
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            //Draw All modules on a row
                            for (int i = 0; i < modulesPerRow && currentIndex < modulesToShow.Count; i++, currentIndex = modulesPerRow * currentRow + i)
                            {
                                if (GUILayout.Button(GetTexture(modulesToShow[currentIndex]), GUILayout.ExpandWidth(true)))
                                    SelectedModuleProperty = modulesToShow[currentIndex];
                            }
                        }
                        currentRow++;
                    }
                }while(currentIndex < modulesToShow.Count-1);
            }
            GUILayout.EndScrollView();
        }

        private Texture GetTexture(ModuleData moduleData)
        {
            HouseModule modulePrefab = moduleData.Module;
            Texture preview = moduleData.Preview;

            if(preview == null)
            {
                preview = AssetPreview.GetAssetPreview(modulePrefab);
                moduleData.Preview = preview;
            }

            return preview;
        }

        private void UpdateAllPreviews(List<ModuleData> modules)
        {
            foreach(ModuleData moduleData in modules)
            {
                Texture texture = AssetPreview.GetAssetPreview(moduleData.Module.gameObject);
                moduleData.Preview = texture;

                EditorUtility.SetDirty(moduleData);
            }
            AssetDatabase.SaveAssets();

            Repaint();
        }

        public void ShowModuleInfo(bool show)
        {
            _showModuleInfo = show;
            Repaint();
        }
    }
}
