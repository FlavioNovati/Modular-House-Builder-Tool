using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    [System.Serializable]
    public class SnappingPoint
    {
        [SerializeField] public bool UseFilter;
        [SerializeField] public Vector3 LocalPoint;
        [SerializeField] public ModuleType SnappingPointFilter;

        public SnappingPoint()
        {
            this.UseFilter = false;
            LocalPoint = Vector3.zero;
            SnappingPointFilter = ModuleType.WALL;
        }

        public SnappingPoint(Vector3 localPos)
        {
            this.UseFilter = false;
            LocalPoint = localPos;
            SnappingPointFilter = ModuleType.WALL;
        }

        public SnappingPoint(Vector3 localPos, ModuleType filter)
        {
            this.UseFilter = true;
            LocalPoint = localPos;
            SnappingPointFilter = filter;
        }
    }
}