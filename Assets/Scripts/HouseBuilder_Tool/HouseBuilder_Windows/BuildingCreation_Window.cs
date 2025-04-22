using UnityEngine;
 
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tool.ModularHouseBuilder.SubTool
{
    public class BuildingCreation_Window : EditorWindow
    {
        public static void OpenWindow()
        {
            string artFolderPath = EditorPrefs.GetString("ModularHouseBuilder_ART_FOLDER");

            //Get Icon Asset
            Texture windowIcon = (Texture)AssetDatabase.LoadAssetAtPath($"{artFolderPath}NewBuildingIcon.png", typeof(Texture));
            GUIContent titleContent = new GUIContent("Building Creator", windowIcon, "Tool to create buildings");

            //Create Window
            BuildingCreation_Window window = GetWindow<BuildingCreation_Window>();
            //Assign Fancy Graphics
            window.titleContent = titleContent;
            window.name = "Building Creation";
        }

        //Building Data
        //Module Amount

        //Buttons in scene
        //Save asset -> Save asset popup window
        //Remove all scripts (Keep two saves)
        //Delete asset -> Delete asset popo window

        //Show Available assets

        //Highlight modules


        //Asset Preview in scene
        //Show overlap box
        //Show snappable moduless

        //Set up Window values
        private void OnEnable()
        {
            //Enter Prefab Mode
            //Edit only on prefab mode

            SceneView.duringSceneGui += DuringSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        //Draw Window
        private void OnGUI()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                this.Close();
                return;
            }


        }

        private void DuringSceneGUI(SceneView sceneView)
        {

        }

    }
}
