using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Tool.ModularHouseBuilder
{
    public class HouseBuilderModule : MonoBehaviour
    {
        public ModuleData ModuleData;
        public ModuleType ModuleType => ModuleData.ModuleType;

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                return;

            //Draw Extension
            Vector3 objectExtension = ModuleData.Extension;
            Vector3 offset = ModuleData.CenterOffset;

            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.DrawWireCube(offset, objectExtension);
        }
        #endif
    }
}
