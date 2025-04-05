using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
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
            //Draw Extension
            Vector3 objectExtension = ModuleData.Extension;
            Vector3 offset = ModuleData.CenterOffset;
            Handles.DrawWireCube(offset, objectExtension);
        }
        #endif
    }
}
