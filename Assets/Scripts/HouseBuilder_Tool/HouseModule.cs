using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Tool.ModularHouseBuilder
{
    public class HouseModule : MonoBehaviour
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
            Vector3 offset = ModuleData.ColliderCenter;
            Matrix4x4 handlesMatrix = Matrix4x4.TRS(transform.position + offset, transform.rotation, Vector3.one);

            Handles.matrix = handlesMatrix;
            Handles.color = ModuleType.ToColor();
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.DrawWireCube(Vector3.zero, objectExtension);
        }
        #endif
    }
}
