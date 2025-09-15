using CityBuilder.Application.Interfaces;
using CityBuilder.Application.Events;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace CityBuilder.Application.UseCases
{
    public class RemoveBuildingUseCase
    {
        private readonly IBuildingRepository _repo;
        private readonly IEventBus _events;

        public RemoveBuildingUseCase(IBuildingRepository repo, IEventBus events) { _repo = repo; _events = events; }


        public async UniTask<bool> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await UniTask.Yield();

            var building = _repo.FindById(id);
            if (building == null) return false;

            _repo.Remove(id);
            _events.Publish(new BuildingRemovedEvent(id));
            return true;
        }
    }
}