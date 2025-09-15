using CityBuilder.Application.Interfaces;
using CityBuilder.Application.Events;
using CityBuilder.Application.DTOs;
using CityBuilder.Domain.Rules;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace CityBuilder.Application.UseCases
{
    public class UpgradeBuildingUseCase
    {
        private readonly IBuildingRepository _repo;
        private readonly IResourceRepository _resources;
        private readonly IEventBus _events;

        public UpgradeBuildingUseCase(IBuildingRepository repo, IResourceRepository resources, IEventBus events) { _repo = repo; _resources = resources; _events = events; }


        public async UniTask<bool> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await UniTask.Yield();
            
            var building = _repo.FindById(id);
            if (building == null) return false;
            
            var nextLevel = UpgradeRules.GetNextLevel(building.Type, building.Level);
            if (nextLevel == null) return false;
            
            if (!await _resources.TrySpendAsync(nextLevel.Cost, cancellationToken)) 
            { 
                _events.Publish(new NotEnoughGoldEvent(nextLevel.Cost)); 
                return false; 
            }
            
            building.UpgradeTo(nextLevel);
            _events.Publish(new BuildingUpgradedEvent(building));
            return true;
        }
    }
}
