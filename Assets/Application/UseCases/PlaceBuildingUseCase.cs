using CityBuilder.Application.Interfaces;
using CityBuilder.Application.Events;
using CityBuilder.Domain.Entities;
using CityBuilder.Domain.Rules;
using Cysharp.Threading.Tasks;
using System.Threading;

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

        public async UniTask<bool> ExecuteAsync(BuildingType type, GridPosition pos, CancellationToken cancellationToken = default)
        {
            if (!_grid.IsCellFree(pos))
            {
                _events.Publish(new CellOccupiedEvent(pos.X, pos.Y));
                return false;
            }

            var defaultLevel = UpgradeRules.GetDefaultLevel(type);

            if (!await _resources.TrySpendAsync(defaultLevel.Cost, cancellationToken))
            {
                _events.Publish(new NotEnoughGoldEvent(defaultLevel.Cost));
                return false;
            }

            var building = new Building(type, pos, defaultLevel);
            _repo.Add(building);
            _events.Publish(new BuildingPlacedEvent(building));
            return true;
        }
    }
}