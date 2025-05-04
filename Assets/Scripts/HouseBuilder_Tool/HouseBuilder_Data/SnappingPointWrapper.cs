using System.Collections.Generic;
using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    [System.Serializable]
    public class SnappingPointWrapper
    {
        [SerializeField] public List<SnappingPoint> Points;
    }
}