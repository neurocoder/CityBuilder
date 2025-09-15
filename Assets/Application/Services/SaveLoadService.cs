using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using CityBuilder.Application.DTOs;
using CityBuilder.Application.Interfaces;
using CityBuilder.Domain.Entities;
using CityBuilder.Application.Events;

namespace CityBuilder.Application.Services
{
    public class SaveLoadService
    {
        private readonly IBuildingRepository _repo;
        private readonly IResourceRepository _resources;
        private readonly ISaveAdapter _adapter;
        private readonly IEventBus _events;

        public SaveLoadService(IBuildingRepository repo, IResourceRepository resources, ISaveAdapter adapter, IEventBus events)
        {
            _repo = repo;
            _resources = resources;
            _adapter = adapter;
            _events = events;
        }

        public async UniTask SaveAsync(CancellationToken cancellationToken = default)
        {
            var gameState = new GameStateDTO
            {
                Buildings = _repo.GetAll()
                    .Select(b => new BuildingDTO
                    {
                        Id = b.Id.ToString(),
                        Type = b.Type,
                        X = b.Position.X,
                        Y = b.Position.Y,
                        Level = b.Level.Level
                    })
                    .ToList(),
                Gold = _resources.Gold
            };

            await _adapter.SaveAsync(gameState, cancellationToken);
            _events.Publish(new GameSavedEvent());
        }

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            _repo.Clear();
            var gameState = await _adapter.LoadAsync(cancellationToken);
            var created = new List<Building>();

            foreach (var d in gameState.Buildings)
            {
                var level = new BuildingLevel(d.Level, 0, 1);
                var building = new Building(d.Type, new GridPosition(d.X, d.Y), level);
                _repo.Add(building);
                created.Add(building);
            }

            await _resources.SetGoldAsync(gameState.Gold, cancellationToken);

            _events.Publish(new GameLoadedEvent(created));
        }
    }
}