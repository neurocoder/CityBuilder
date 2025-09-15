using CityBuilder.Application.Interfaces;
using CityBuilder.Application.Events;
using CityBuilder.Domain.Entities;
using System;

namespace CityBuilder.Application.UseCases
{
    public class MoveBuildingUseCase
    {
        private readonly IBuildingRepository _repo;
        private readonly IGridService _grid;
        private readonly IEventBus _events;

        public MoveBuildingUseCase(IBuildingRepository repo, IGridService grid, IEventBus events)
        {
            _repo = repo; _grid = grid; _events = events;
        }

        public bool Execute(Guid id, GridPosition newPos)
        {
            var b = _repo.FindById(id);
            if (b == null) return false;
            if (!_grid.IsCellFree(newPos)) { _events.Publish(new CellOccupiedEvent(newPos.X, newPos.Y)); return false; }
            b.MoveTo(newPos);
            _events.Publish(new BuildingMovedEvent(b));
            return true;
        }
    }
}
