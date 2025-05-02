using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Tool.ModularHouseBuilder
{
    public class ModuleData : ScriptableObject
    {
        public HouseModule Module;
        public Vector3 Extension;
        public Vector3 Rotation;
        public Vector3 ColliderCenter;

        public string PrefabAssetPath;
        public string ModuleName;

        public ModuleType ModuleType;
        public Texture Preview;

        public List<SnappingPoint> SnappingPoints => _snappingPoints.Points;
        [SerializeField] private SnappingPointWrapper _snappingPoints;

        public Vector3 GetLocalSnappingPosition(Vector3 pos) => GetClosestSnappingPoint(pos, SnappingPoints);

        public Vector3 GetLocalSnappingPosition(Vector3 pos, ModuleType moduleTypeFilter)
        {
            List<SnappingPoint> filteredPoints = SnappingPoints;
            for (int i = 0; i < filteredPoints.Count; i++)
            {
                SnappingPoint point = filteredPoints[i];
                if (point.UseFilter && moduleTypeFilter != point.SnappingPointFilter)
                    filteredPoints.Remove(point);
            }

            return GetClosestSnappingPoint(pos, filteredPoints);
        }

        private Vector3 GetClosestSnappingPoint(Vector3 pos, List<SnappingPoint> points)
        {
            float dist = float.MaxValue;
            SnappingPoint closestPoint = new SnappingPoint();

            foreach (SnappingPoint point in points)
            {
                float distance = (point.LocalPoint - pos).magnitude;
                if (distance < dist)
                {
                    closestPoint = point;
                    dist = distance;
                }
            }

            if(dist == float.MaxValue)
            {
                Debug.LogWarning("NO SNAPPING POINT AVAILABLE FOR THIS OBJECT");
                return Vector3.zero;
            }

            return closestPoint.LocalPoint;
        }

        public void SetSnappingPointsData(List<SnappingPoint> snappingPoints) => _snappingPoints.Points = snappingPoints;
    }

    [System.Serializable]
    public struct SnappingPointWrapper
    {
        public List<SnappingPoint> Points;
    }

    [System.Serializable]
    public struct SnappingPoint
    {
        public bool UseFilter;
        public Vector3 LocalPoint;
        public ModuleType SnappingPointFilter;

        public SnappingPoint(bool useFilter = false)
        {
            this.UseFilter = useFilter;
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