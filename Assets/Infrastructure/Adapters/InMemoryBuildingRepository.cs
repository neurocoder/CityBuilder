using System;
using System.Collections.Generic;
using System.Linq;
using CityBuilder.Application.Interfaces;
using CityBuilder.Domain.Entities;

namespace CityBuilder.Infrastructure.Adapters
{
    public class InMemoryBuildingRepository : IBuildingRepository
    {
        private readonly Dictionary<Guid, Building> _map = new();

        public void Add(Building building) => _map[building.Id] = building;
        public void Remove(Guid id) => _map.Remove(id);
        public Building? FindByPosition(GridPosition pos) => _map.Values.FirstOrDefault(b => b.Position.Equals(pos));
        public Building? FindById(Guid id) => _map.TryGetValue(id, out var b) ? b : null;
        public IEnumerable<Building> GetAll() => _map.Values;
        public void Clear() => _map.Clear();
    }
}
