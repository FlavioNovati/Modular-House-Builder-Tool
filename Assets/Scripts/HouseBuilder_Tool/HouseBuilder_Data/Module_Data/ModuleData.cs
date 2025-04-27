using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    [CreateAssetMenu(fileName = "New Module", menuName = "scriptable/Tools/ModularHouseBuilder/Module")]
    public class ModuleData : ScriptableObject
    {
        public HouseBuilderModule Module;
        public Vector3 Extension;
        public Vector3 Rotation;
        public Vector3 CenterOffset;
        public string PrefabAssetPath;
        public string ModuleName;

        public ModuleType ModuleType;
        public Texture Preview;


        public Vector3 GetSnappingPos(Vector3 pos)
        {
            return Vector3.zero;
        }
    }
}