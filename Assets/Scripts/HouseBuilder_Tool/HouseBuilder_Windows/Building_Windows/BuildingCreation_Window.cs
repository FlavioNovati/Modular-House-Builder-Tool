using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tool.ModularHouseBuilder.SubTool
{
    public class BuildingCreation_Window : EditorWindow
    {
        const string BUILDING_CONTENT_FOLDER = "Buildings";
        const string BUILDING_PREFIX = "Building_";
        private string _defaultContentPath;
        
        public static void OpenWindow(Type dockNextTo)
        {
            string artFolderPath = EditorPrefs.GetString("ModularHouseBuilder_ART_FOLDER");

            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{artFolderPath}NewBuildingIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Building Creator", windowIcon, "Tool to create buildings");

            //Create Window
            BuildingCreation_Window window = GetWindow<BuildingCreation_Window>(dockNextTo);
            //Assign Fancy Graphics
            window.titleContent = titleContent;
            window.name = "Building Creation";
        }

        private string _buildingName;
        private string _buildingDescription;
        private string _buildingPath;

        private void OnEnable()
        {
            string prefabFolder = EditorPrefs.GetString("ModularHouseBuilder_PREFAB_FOLDER");
            _defaultContentPath = $"{prefabFolder}/{BUILDING_CONTENT_FOLDER}/";

            //Create folder if not present
            if (!AssetDatabase.IsValidFolder(_defaultContentPath))
                AssetDatabase.CreateFolder(prefabFolder, BUILDING_CONTENT_FOLDER);

            //Save Building folder path
            if (!EditorPrefs.HasKey("ModularHouseBuilder_BUILDINGS_FOLDER"))
                EditorPrefs.SetString("ModularHouseBuilder_BUILDINGS_FOLDER", _defaultContentPath);
            
            //Apply Deault values
            _buildingName = "New_Building";
            _buildingDescription = "New Building";
            _buildingPath = _defaultContentPath;
        }

        //Draw Windows
        private void OnGUI()
        {
            //Building Name
            GUILayout.Label("Name");
            string newName = GUILayout.TextField(_buildingName, GUILayout.ExpandWidth(true));
            if(newName != _buildingName)
                _buildingName = _buildingName.Replace(' ', '_');
            GUILayout.Space(10f);

            //Building Description
            GUILayout.Label("Description");
            _buildingDescription = GUILayout.TextArea(_buildingDescription, GUILayout.ExpandWidth(true));
            GUILayout.Space(10f);
            
            //Building Path
            GUILayout.Label("Building BuildingPath: "+_buildingPath, GUILayout.ExpandWidth(true));
            GUILayout.Space(5f);
            //Change Asset final Path
            if (GUILayout.Button("Select Asset Folder", GUILayout.ExpandWidth(true)))
                UpdatePath();

            //Create
            if (GUILayout.Button("Create", GUILayout.ExpandWidth(true)))
                CreateBuildingPrefab();

            HandleInput();
        }

        private void UpdatePath()
        {
            //Get Final Path
            string newPath = EditorUtility.OpenFolderPanel("Select Destination Folder", _defaultContentPath, "");

            //Check if the folder is not selected
            if(string.IsNullOrEmpty(newPath))
                return;

            //Add / to select the folder
            newPath += "/";

            //Remove all unuseful content
            string delimiter = "Assets/";
            int delimiterIndex;
            delimiterIndex = newPath.IndexOf(delimiter);

            //Check if folder is in "Assets"
            if (delimiterIndex > 0)
                _buildingPath = newPath.Substring(delimiterIndex);
            else
                EditorUtility.DisplayDialog("PATH NOT AVAILABLE", "Select a folder inside Asset folder", "Ok");
        }

        private void CreateBuildingPrefab()
        {
            //Building params
            string path = $"{_buildingPath}{BUILDING_PREFIX}{_buildingName}.prefab";
            string buildingPath = AssetDatabase.GenerateUniqueAssetPath(path);
            string buildingName = $"{BUILDING_PREFIX}{_buildingName}";
            //Building Data params
            string dataPath = $"{_buildingPath}{BUILDING_PREFIX}{_buildingName}.asset";
            dataPath = AssetDatabase.GenerateUniqueAssetPath(dataPath);
            
            //Create Prefab
            GameObject buildingPrefab = new GameObject(buildingName, typeof(Building));
            PrefabUtility.SaveAsPrefabAsset(buildingPrefab, buildingPath);
            //Destroy instanciated gameObject in scene
            DestroyImmediate(buildingPrefab);

            //Create Scriptable
            BuildingData buildingData = ScriptableObject.CreateInstance<BuildingData>();
            buildingData.Name = _buildingName;
            buildingData.Description = _buildingDescription;
            buildingData.BuildingPath = buildingPath;

            AssetDatabase.CreateAsset(buildingData, dataPath);

            //Update prefab local varable
            buildingPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(buildingPath, typeof(GameObject));

            //Link Data
            buildingPrefab.GetComponent<Building>().BuildingData = buildingData;
            buildingData.Building = buildingPrefab.GetComponent<Building>();

            //Save Prefab
            PrefabUtility.SavePrefabAsset(buildingPrefab);
            //Save Scriptable
            EditorUtility.SetDirty(buildingData);


            //Enter Prefab Mode
            PrefabStageUtility.OpenPrefab(buildingPath);

            //Set Building Focus
            //Open Building Edit Mode
        }

        private void HandleInput()
        {
            //Lose Focus
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                GUI.FocusControl(null);
                Event.current.Use();
                Repaint();
            }
        }
    }
}
