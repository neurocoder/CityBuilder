using CityBuilder.Application.Interfaces;
using CityBuilder.Application.Events;
using CityBuilder.Domain.Entities;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

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


        public async UniTask<bool> ExecuteAsync(Guid id, GridPosition newPos, CancellationToken cancellationToken = default)
        {
            await UniTask.Yield();

            var building = _repo.FindById(id);
            if (building == null) return false;

            if (!_grid.IsCellFree(newPos))
            {
                _events.Publish(new CellOccupiedEvent(newPos.X, newPos.Y));
                return false;
            }

            building.MoveTo(newPos);
            _events.Publish(new BuildingMovedEvent(building));
            return true;
        }
    }
}