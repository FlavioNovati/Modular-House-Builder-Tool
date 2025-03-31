using UnityEditor;
using UnityEngine;

using Tool.ModularHouseBuilder.SubTool;

namespace Tool.ModularHouseBuilder
{
    public class ModularHouseBuilder : EditorWindow
    {
        [MenuItem("Tools/House Builder")]
        public static void OpenHouseBuilder()
        {
            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Scripts/HouseBuilder_Tool/HouseBuilder_Art/MainIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Modular House Builder", windowIcon, "Tool to create modular structures");

            //Create Window
            ModularHouseBuilder window = GetWindow<ModularHouseBuilder>();
            //Assign Fancy Graphics
            window.titleContent = titleContent;
            window.name = "House Builder";
        }

        private void OnEnable()
        {
            //this.BeginWindows();
        }

        private void OnDisable()
        {
            //this.EndWindows();
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Create New Module", GUILayout.ExpandWidth(true)))
                ModuleCreation_Window.OpenModuleCreation_Window(typeof(ModularHouseBuilder));

            if(GUILayout.Button("Create New Building", GUILayout.ExpandWidth(true)))
            {
                
            }
        }
    }
}