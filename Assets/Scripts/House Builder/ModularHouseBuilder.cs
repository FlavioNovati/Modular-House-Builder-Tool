using UnityEditor;
using UnityEngine;

using Tool.ModularHouseBuilder.SubTool;

namespace Tool.ModularHouseBuilder
{
    public class ModularHouseBuilder : EditorWindow
    {
        [MenuItem("Tools/House Builder")] public static void OpenHouseBuilder() => GetWindow<ModularHouseBuilder>("Modular House Builder");

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Create New Module", GUILayout.ExpandWidth(true)))
                ModuleCreation_Window.OpenModuleCreation_Window();

            if(GUILayout.Button("Create New Building", GUILayout.ExpandWidth(true)))
            {
                
            }
        }
    }
}