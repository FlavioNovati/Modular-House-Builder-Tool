using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    public class BuildingData : ScriptableObject
    {
        public string Name;
        public string Description;
        public string Path;
        public Building Building;

        public int ModulesInBuilding;

        public BuildingData()
        {
            Name = string.Empty;
            Description = string.Empty;
            Path = string.Empty;
            Building = null;

            ModulesInBuilding = 0;
        }
    }
}
