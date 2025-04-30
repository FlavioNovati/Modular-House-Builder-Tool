using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

using Tool.ModularHouseBuilder.SubTool;

namespace Tool.ModularHouseBuilder
{
    [CustomEditor(typeof(BuildingData))]
    public class BuildingData_Editor : Editor
    {
        private GUIStyle _buildingNameStyle;
        private BuildingData _buildingData;

        private int _modulesTypesAmount;
        private GUIStyle _moduleTitleStyle;

        private Building BuildinLinkdedToData;

        void OnEnable()
        {
            //Get Building Data
            _buildingData = (BuildingData)target;

            _buildingNameStyle = new GUIStyle()
            {
                fontSize = 30,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _buildingNameStyle.normal.textColor = Color.white;

            //Modules params
            _modulesTypesAmount = Enum.GetNames(typeof(ModuleType)).Length;
            _moduleTitleStyle = new GUIStyle()
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
            };
            _moduleTitleStyle.normal.textColor = Color.white;

            BuildinLinkdedToData = (Building)AssetDatabase.LoadAssetAtPath(_buildingData.BuildingPath, typeof(Building));
        }

        public override void OnInspectorGUI()
        {
            //Building Generic Info
            ShowNameAndDescrition();

            if (GUILayout.Button("Open in Building Editor", GUILayout.ExpandWidth(true))) //Enter Prefab Mode
            {
                //PrefabStageUtility.OpenPrefab(_buildingData.BuildingPath);
                //Open Editor Window
                BuildingEditor_Window.OpenWindow(_buildingData.Building);
            }

            //Modules Count
            ShowModulesCount();

        }

        private void ShowNameAndDescrition()
        {
            GUILayout.Label(_buildingData.BuildingName, _buildingNameStyle);
            GUILayout.Space(5f);
            GUILayout.Label(_buildingData.Description, GUILayout.ExpandWidth(true));
            GUILayout.Space(5f);
        }

        private void ShowModulesCount()
        {
            GUILayout.Label("Building Info", _moduleTitleStyle, GUILayout.ExpandWidth(true));

            int modulesCount = _buildingData.ModulesInBuilding;
            GUILayout.Label($"Modules Count: {modulesCount}", GUILayout.ExpandWidth(true));
            GUILayout.Space(5f);

            //Show Specific Modules Count
            for (int i = 0; i < _modulesTypesAmount; i++)
            {
                ModuleType type = ModuleTypeUtils.ModuleTypeFromInt(i);
                string typeName = type.ToFormattedName();
                int typeInsideBuinding = _buildingData.GetModules(type).Count;

                GUILayout.Label($"\t- {typeName}{typeInsideBuinding}", GUILayout.ExpandWidth(true));
            }
        }
    }
}
