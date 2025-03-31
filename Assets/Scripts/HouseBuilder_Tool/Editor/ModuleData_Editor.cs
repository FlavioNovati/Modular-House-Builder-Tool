using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    [CustomEditor(typeof(ModuleData))]
    public class ModuleData_Editor : Editor
    {
        private ModuleData _moduleData;

        const float TEXTURE_MAX_SIZE = 150f;

        void OnEnable()
        {
            _moduleData = (ModuleData)target;
            _moduleData.SetPreview(AssetPreview.GetAssetPreview(_moduleData.Prefab));

            SceneView.duringSceneGui += DuringSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            float rectWidth = EditorGUIUtility.currentViewWidth;

            //Title
            GUIContent typeTexture = new GUIContent(" Preview", _moduleData.ModuleType.ToTexture());
            GUILayout.Label(typeTexture, GUILayout.ExpandWidth(true));

            //Draw Preview
            GUIContent textureContent = new GUIContent(_moduleData.ModulePreview);
            //Preview Settings
            GUILayoutOption[] textureOptions =
            {
                GUILayout.Height(_moduleData.ModulePreview.height),
                GUILayout.Width(_moduleData.ModulePreview.width),
            };

            //Draw Preview as a label
            GUILayout.Label(textureContent, textureOptions);

            //Open Prefab Button
            if (GUILayout.Button("Open Prefab", GUILayout.ExpandWidth(true)))
            {
                string prefabPath = AssetDatabase.GetAssetPath(_moduleData.Prefab);
                PrefabStageUtility.OpenPrefab(prefabPath);
            }
        }

        private void DuringSceneGUI(SceneView view)
        {
            //Draw Gizsmo
            //Draw Collision Shape
            //Add Collision?
            //Draw Collision that will be removed
            
            //Confirm
        }
    }
}