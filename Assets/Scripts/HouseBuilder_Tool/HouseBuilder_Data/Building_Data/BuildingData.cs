using UnityEngine;

using System.Collections.Generic;
using System.Linq;


namespace Tool.ModularHouseBuilder
{
    public class BuildingData : ScriptableObject
    {
        public string Name;
        public string Description;
        public string BuildingPath;
        public Building Building;
        public Texture Preview;

        public int ModulesInBuilding;
        private List<ModuleData> _modules;

        public BuildingData()
        {
            Name = string.Empty;
            Description = string.Empty;
            BuildingPath = string.Empty;

            Building = null;
            Preview = null;

            ModulesInBuilding = 0;

            _modules = new List<ModuleData>();
        }

        public void AddModule(ModuleData moduleToAdd) => _modules.Add(moduleToAdd);
        public bool RemoveModule(ModuleData moduleToRemove) => _modules.Remove(moduleToRemove);

        public List<ModuleData> GetModules() => _modules;
        public List<ModuleData> GetModules(ModuleType moduleType) => _modules.Where<ModuleData>(module => (module.ModuleType == moduleType)).ToList();
    }
}
