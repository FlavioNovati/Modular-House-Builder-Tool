using UnityEditor;
using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    [CreateAssetMenu(fileName = "New Module", menuName = "scriptable/Tools/ModularHouseBuilder/Module")]
    public class ModuleData : ScriptableObject
    {
        public HouseModule Module;
        public Vector3 Extension;
        public Vector3 Rotation;
        public Vector3 CenterOffset;

        public string PrefabAssetPath;
        public string ModuleName;

        public ModuleType ModuleType;
        public Texture Preview;

        public Vector3 CenterPoint => Extension / 2f;

        public Vector3 GetLocalSnappingPos(Vector3 pos, ModuleType moduleTypeFilter)
        {
            switch (moduleTypeFilter)
            {
                case ModuleType.WALL:
                    break;

                case ModuleType.WALL_DECORATOR:
                    break;

                case ModuleType.DOOR:
                    break;

                case ModuleType.DOOR_FRAME:
                    break;

                case ModuleType.FLOOR:
                    break;

                case ModuleType.FLOOR_DECORATOR:
                    break;

                case ModuleType.WINDOW:
                    break;

                case ModuleType.WINDOW_FRAME:
                    break;

                case ModuleType.ROOF:
                    break;
            }

            return Vector3.zero;
        }


        private void GenerateLocalSnappingPositions()
        {
            Vector3[] snappingPositions = new Vector3[16];
        }
    }
}