using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    public static class ModuleData_ExtentionMethods
    {
        //Could be done better with Bound class, it's too late to change everything
        public static void CreateSnappingPoints(this ModuleData moduleData)
        {
            //Generic cases

            //All local
            Vector3 extension = moduleData.Extension;
            Vector3 center = moduleData.ColliderCenter;
            Vector3 objectOrigin = center;
            objectOrigin.y = 0;

            float height = extension.y;
            float width = extension.x;
            float depth = extension.z;

            List<SnappingPoint> points = new List<SnappingPoint>(8);

            //Get lower points
            SnappingPoint[] lowerPoints = new SnappingPoint[4];

            Vector3 frwPos = center + (Vector3.forward * depth / 2f);
            frwPos.y -= height / 2f;
            lowerPoints[0] = new SnappingPoint(frwPos, ModuleType.WALL);

            Vector3 rxPos = center + (Vector3.right * width / 2f);
            rxPos.y -= height / 2f;
            lowerPoints[1] = new SnappingPoint(rxPos, ModuleType.WALL);

            Vector3 backPos = center - (Vector3.forward * depth / 2f);
            backPos.y -= height / 2f;
            lowerPoints[2] = new SnappingPoint(backPos, ModuleType.WALL);

            Vector3 sxPos = center - (Vector3.right * width / 2f);
            sxPos.y -= height / 2f;
            lowerPoints[3] = new SnappingPoint(sxPos, ModuleType.WALL);

            //Get Upper points
            SnappingPoint[] upperPoints = new SnappingPoint[4];

            Vector3 frwUPos = frwPos + (Vector3.up * height);
            upperPoints[0] = new SnappingPoint(frwUPos, ModuleType.WALL);

            Vector3 rxUPos = rxPos + (Vector3.up * height);
            upperPoints[1] = new SnappingPoint(rxUPos, ModuleType.WALL);

            Vector3 backUPos = backPos + (Vector3.up * height);
            upperPoints[2] = new SnappingPoint(backUPos, ModuleType.WALL);

            Vector3 sxUPos = sxPos + (Vector3.up * height);
            upperPoints[3] = new SnappingPoint(sxUPos, ModuleType.WALL);

            //Add Snapping positions
            points.AddRange(lowerPoints);
            points.AddRange(upperPoints);

            //Additive cases
            switch (moduleData.ModuleType)
            {
                case ModuleType.DOOR:
                    points.Clear();
                    points.Add(new SnappingPoint(objectOrigin, ModuleType.DOOR));
                    break;

                case ModuleType.DOOR_FRAME:
                    points.Add(new SnappingPoint(objectOrigin, ModuleType.DOOR));
                    break;

                case ModuleType.WINDOW:
                    points.Clear();
                    points.Add(new SnappingPoint(objectOrigin, ModuleType.WINDOW));
                    break;

                case ModuleType.WINDOW_FRAME:
                    points.Add(new SnappingPoint(center, ModuleType.WINDOW));
                    break;
            }

            //Set Points
            moduleData.SetSnappingPointsData(points);
            moduleData.Save();
        }
    }
}