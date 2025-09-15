using CityBuilder.Application.Interfaces;
using CityBuilder.Application.Events;
using CityBuilder.Application.DTOs;
using CityBuilder.Domain.Entities;
using CityBuilder.Domain.Rules;
using System;

namespace CityBuilder.Application.UseCases
{
    public class PlaceBuildingUseCase
    {
        private readonly IBuildingRepository _repo;
        private readonly IGridService _grid;
        private readonly IResourceRepository _resources;
        private readonly IEventBus _events;

        public PlaceBuildingUseCase(IBuildingRepository repo, IGridService grid, IResourceRepository resources, IEventBus events)
        {
            _repo = repo; _grid = grid; _resources = resources; _events = events;
        }

        public bool Execute(BuildingType type, GridPosition pos)
        {
            if (!_grid.IsCellFree(pos)) { _events.Publish(new CellOccupiedEvent(pos.X, pos.Y)); return false; }
            var defaultLevel = UpgradeRules.GetDefaultLevel(type);
            if (!_resources.TrySpend(defaultLevel.Cost)) { _events.Publish(new NotEnoughGoldEvent(defaultLevel.Cost)); return false; }
            var b = new Building(type, pos, defaultLevel);
            _repo.Add(b);
            _events.Publish(new BuildingPlacedEvent(b));
            return true;
        }

        public bool Execute(PlaceBuildingDTO dto)
        {
            var pos = new GridPosition(dto.X, dto.Y);
            return Execute(dto.Type, pos);
        }
    }
}
