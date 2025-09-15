using CityBuilder.Application.Interfaces;
using CityBuilder.Application.Events;
using CityBuilder.Application.DTOs;
using CityBuilder.Domain.Rules;
using System;

namespace CityBuilder.Application.UseCases
{
    public class UpgradeBuildingUseCase
    {
        private readonly IBuildingRepository _repo;
        private readonly IResourceRepository _resources;
        private readonly IEventBus _events;

        public UpgradeBuildingUseCase(IBuildingRepository repo, IResourceRepository resources, IEventBus events) { _repo = repo; _resources = resources; _events = events; }

        public bool Execute(Guid id)
        {
            var b = _repo.FindById(id);
            if (b == null) return false;
            var next = UpgradeRules.GetNextLevel(b.Type, b.Level);
            if (next == null) return false;
            if (!_resources.TrySpend(next.Cost)) { _events.Publish(new NotEnoughGoldEvent(next.Cost)); return false; }
            b.UpgradeTo(next);
            _events.Publish(new BuildingUpgradedEvent(b));
            return true;
        }

        public bool Execute(UpgradeBuildingDTO dto) { return Execute(dto.BuildingId); }
    }
}
