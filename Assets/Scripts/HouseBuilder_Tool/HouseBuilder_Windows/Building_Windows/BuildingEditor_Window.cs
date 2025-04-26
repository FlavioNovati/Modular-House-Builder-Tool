using UnityEditor;
using UnityEngine;

namespace Tool.ModularHouseBuilder.SubTool
{
    public class BuildingEditor_Window : EditorWindow
    {
        private static ModulesExplorer_Window s_moduleExplorer;

        public static void OpenWindow()
        {
            string artFolderPath = EditorPrefs.GetString("ModularHouseBuilder_ART_FOLDER");

            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{artFolderPath}BuildingEditIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Building Editor", windowIcon, "Tool to edit buildings");
            
            BuildingEditor_Window window = GetWindow<BuildingEditor_Window>();
            window.titleContent = titleContent;

            //Open Module Displayer 
            s_moduleExplorer = GetWindow<ModulesExplorer_Window>();
            s_moduleExplorer.ShowModuleInfo(false);
        }

        private void OnEnable()
        {
            if(s_moduleExplorer ==  null)
            {
                Close();
                return;
            }
        }

        //Buttons in scene
        //Save asset -> Save asset popup window
        //Remove all scripts (Keep two saves)
        //Delete asset -> Delete asset popo window

        //Show Available assets

        //Highlight modules

        //Enter Prefab Mode
        //Edit only on prefab mode

        //Asset Preview in scene
        //Show overlap box
        //Show snappable moduless

        //Set up Window values
    }
}
