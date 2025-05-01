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

        public SnappingPoint[] SnappingPoints => _snappingPoints.SnappingPoints;
        private SnappingPointsData _snappingPoints;
        
        public Vector3 GetLocalSnappingPosition(Vector3 pos) => GetClosestSnappingPoint(pos, _snappingPoints.SnappingPoints);

        public Vector3 GetLocalSnappingPosition(Vector3 pos, ModuleType moduleTypeFilter)
        {
            SnappingPoint[] filteredPoints = _snappingPoints.SnappingPoints.Where<SnappingPoint>(snapPoint => snapPoint.SnappingPointFilter == moduleTypeFilter || !snapPoint.UseFilter) as SnappingPoint[];
            return GetClosestSnappingPoint(pos, filteredPoints);
        }

        private Vector3 GetClosestSnappingPoint(Vector3 pos, SnappingPoint[] points)
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

        public void SetSnappingPointsData(SnappingPointsData snappingPointsData) => _snappingPoints = snappingPointsData;
    }

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

    public struct SnappingPointsData
    {
        public SnappingPoint[] SnappingPoints;
    }
}