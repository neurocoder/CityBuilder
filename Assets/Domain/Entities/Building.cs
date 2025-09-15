using System;

namespace CityBuilder.Domain.Entities
{
    public sealed class Building
    {
        public Guid Id { get; }
        public BuildingType Type { get; }
        public GridPosition Position { get; private set; }
        public BuildingLevel Level { get; private set; }

        public Building(BuildingType type, GridPosition position, BuildingLevel level)
        {
            Id = Guid.NewGuid();
            Type = type;
            Position = position;
            Level = level;
        }

        public void MoveTo(GridPosition newPosition) => Position = newPosition;
        public void UpgradeTo(BuildingLevel newLevel)
        {
            if (newLevel.Level <= Level.Level) throw new InvalidOperationException("New level must be greater than current level.");
            Level = newLevel;
        }
    }
}
