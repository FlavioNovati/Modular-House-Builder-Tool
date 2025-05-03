using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

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

        [SerializeField] public List<SnappingPoint> SnappingPoints
        {
            get => _snappingPoints.Points;
            set => _snappingPoints.Points = value;
        }

        [SerializeField] public SnappingPointWrapper _snappingPoints;

        public Vector3 GetLocalSnappingPosition(Vector3 pos, Quaternion rot) => GetClosestSnappingPoint(pos, rot, SnappingPoints);

        public Vector3 GetLocalSnappingPosition(Vector3 pos, Quaternion rot, ModuleType moduleTypeFilter)
        {
            //Filte Snap Positions
            List<SnappingPoint> filteredPoints = new List<SnappingPoint>();
            filteredPoints.AddRange(SnappingPoints);

            foreach (SnappingPoint point in SnappingPoints)
            {
                if (point.UseFilter && point.SnappingPointFilter != moduleTypeFilter)
                    filteredPoints.Remove(point);
            }

            return GetClosestSnappingPoint(pos, rot, filteredPoints);
        }

        private Vector3 GetClosestSnappingPoint(Vector3 pos, Quaternion rot, List<SnappingPoint> points)
        {
            if(points.Count <= 0)
                return pos;

            List<SnappingPoint> rotatedSnappingPoints = new List<SnappingPoint>();
            for(int i=0; i<points.Count; i++)
            {
                SnappingPoint rotatedPoint = points[i];
                rotatedPoint.LocalPoint = rot * rotatedPoint.LocalPoint;

                rotatedSnappingPoints.Add(rotatedPoint);
            }

            float dist = float.MaxValue;
            SnappingPoint closestPoint = new SnappingPoint();

            foreach (SnappingPoint point in rotatedSnappingPoints)
            {
                float distance = (pos - point.LocalPoint).magnitude;
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
}