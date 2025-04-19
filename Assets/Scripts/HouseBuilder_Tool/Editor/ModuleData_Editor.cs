using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tool.ModularHouseBuilder
{
    [CustomEditor(typeof(ModuleData))]
    public class ModuleData_Editor : Editor
    {
        private ModuleData _moduleData;

        void OnEnable()
        {
            _moduleData = (ModuleData)target;
        }

        public override void OnInspectorGUI()
        {
            float rectWidth = EditorGUIUtility.currentViewWidth;

            //Title
            GUIContent typeTexture = new GUIContent(" Preview", _moduleData.ModuleType.ToTexture());
            GUILayout.Label(typeTexture, GUILayout.ExpandWidth(true));

            if (_moduleData.Preview != null)
            {
                //Draw Preview
                GUIContent textureContent = new GUIContent(_moduleData.Preview);
                //Preview Settings
                GUILayoutOption[] textureOptions =
                {
                    GUILayout.Height(_moduleData.Preview.height),
                    GUILayout.Width(_moduleData.Preview.width),
                };

                //Draw Preview as a label
                GUILayout.Label(textureContent, textureOptions);
            }
            else
                GUILayout.Label("NO PREVIEW AVAILABLE");

            //Open Prefab Button
            if (GUILayout.Button("Open Prefab", GUILayout.ExpandWidth(true)))
            {
                string prefabPath = AssetDatabase.GetAssetPath(_moduleData.Prefab);
                PrefabStageUtility.OpenPrefab(prefabPath);
            }
        }
    }
}