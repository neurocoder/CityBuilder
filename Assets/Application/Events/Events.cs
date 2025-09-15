using CityBuilder.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CityBuilder.Application.Events
{
    public sealed class BuildingPlacedEvent { public Building Building { get; } public BuildingPlacedEvent(Building b) => Building = b; }
    public sealed class BuildingRemovedEvent { public Guid BuildingId { get; } public BuildingRemovedEvent(Guid id) => BuildingId = id; }
    public sealed class BuildingMovedEvent { public Building Building { get; } public BuildingMovedEvent(Building b) => Building = b; }
    public sealed class BuildingUpgradedEvent { public Building Building { get; } public BuildingUpgradedEvent(Building b) => Building = b; }
    public sealed class NotEnoughGoldEvent { public int Required { get; } public NotEnoughGoldEvent(int r) => Required = r; }
    public sealed class CellOccupiedEvent { public int X { get; } public int Y { get; } public CellOccupiedEvent(int x,int y){X=x;Y=y;} }
    public sealed class GameSavedEvent { }
    public sealed class GameLoadedEvent { public IReadOnlyList<Domain.Entities.Building> Buildings { get; } public GameLoadedEvent(IReadOnlyList<Domain.Entities.Building> b) => Buildings = b; }
}
