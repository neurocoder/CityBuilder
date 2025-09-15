using System.Collections.Generic;
using System.Linq;
using CityBuilder.Application.DTOs;
using CityBuilder.Application.Interfaces;
using CityBuilder.Domain.Entities;
using CityBuilder.Application.Events;

namespace CityBuilder.Application.Services
{
    public class SaveLoadService
    {
        private readonly IBuildingRepository _repo;
        private readonly ISaveAdapter _adapter;
        private readonly IEventBus _events;

        public SaveLoadService(IBuildingRepository repo, ISaveAdapter adapter, IEventBus events)
        {
            _repo = repo;
            _adapter = adapter;
            _events = events;
        }

        public void Save()
        {
            var dto = _repo.GetAll()
                .Select(b => new BuildingDTO
                {
                    Id = b.Id.ToString(),
                    Type = b.Type,
                    X = b.Position.X,
                    Y = b.Position.Y,
                    Level = b.Level.Level
                })
                .ToList();

            _adapter.Save(dto);
            _events.Publish(new GameSavedEvent());
        }

        public void Load()
        {
            _repo.Clear();
            var list = _adapter.Load();
            var created = new List<Building>();
            foreach (var d in list)
            {
                var level = new BuildingLevel(d.Level, 0, 1);
                var b = new Building(d.Type, new GridPosition(d.X, d.Y), level);
                _repo.Add(b);
                created.Add(b);
            }

            _events.Publish(new GameLoadedEvent(created));
        }
    }
}
