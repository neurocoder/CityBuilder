using CityBuilder.Application.Interfaces;
using CityBuilder.Application.Events;
using System;

namespace CityBuilder.Application.UseCases
{
    public class RemoveBuildingUseCase
    {
        private readonly IBuildingRepository _repo;
        private readonly IEventBus _events;

        public RemoveBuildingUseCase(IBuildingRepository repo, IEventBus events) { _repo = repo; _events = events; }

        public bool Execute(Guid id) { var b = _repo.FindById(id); if (b == null) return false; _repo.Remove(id); _events.Publish(new BuildingRemovedEvent(id)); return true; }
    }
}
