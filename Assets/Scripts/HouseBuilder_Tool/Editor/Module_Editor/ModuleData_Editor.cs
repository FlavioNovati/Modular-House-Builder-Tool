using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tool.ModularHouseBuilder
{
    [CustomEditor(typeof(ModuleData))]
    public class ModuleData_Editor : Editor
    {
        private ModuleData _moduleData;
        private GUIStyle _moduleNameStyle;

        void OnEnable()
        {
            _moduleData = (ModuleData)target;

            _moduleNameStyle = new GUIStyle()
            {
                fontSize = 30,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _moduleNameStyle.normal.textColor = Color.white;
        }

        public override void OnInspectorGUI()
        {
            float rectWidth = EditorGUIUtility.currentViewWidth;

            //Title
            GUILayout.Label(_moduleData.ModuleName, _moduleNameStyle);
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
                _moduleData.Preview = AssetPreview.GetAssetPreview(_moduleData.Module.gameObject);

            //Open Prefab Button
            if (GUILayout.Button("OpenWindow Module", GUILayout.ExpandWidth(true)))
            {
                string prefabPath = AssetDatabase.GetAssetPath(_moduleData.Module);
                PrefabStageUtility.OpenPrefab(prefabPath);
            }
        }
    }
}