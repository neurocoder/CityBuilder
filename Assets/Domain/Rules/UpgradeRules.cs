using System.Collections.Generic;
using CityBuilder.Domain.Entities;

namespace CityBuilder.Domain.Rules
{
    public static class UpgradeRules
    {
        private static readonly Dictionary<BuildingType, Dictionary<int, BuildingLevel>> _levels = new()
        {
            { BuildingType.House, new() { {1, new BuildingLevel(1,10,1)}, {2, new BuildingLevel(2,50,2)} } },
            { BuildingType.Farm, new() { {1, new BuildingLevel(1,15,2)}, {2, new BuildingLevel(2,80,4)} } },
            { BuildingType.Mine, new() { {1, new BuildingLevel(1,20,3)}, {2, new BuildingLevel(2,120,6)} } }
        };

        public static BuildingLevel GetDefaultLevel(BuildingType type) => _levels[type][1];
        public static BuildingLevel? GetNextLevel(BuildingType type, BuildingLevel current)
        {
            var next = current.Level + 1;
            return _levels.TryGetValue(type, out var table) && table.TryGetValue(next, out var lvl) ? lvl : null;
        }
    }
}