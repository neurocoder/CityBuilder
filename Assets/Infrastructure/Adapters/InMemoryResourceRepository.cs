using CityBuilder.Application.Interfaces;

namespace CityBuilder.Infrastructure.Adapters
{
    public class InMemoryResourceRepository : IResourceRepository
    {
        private int _gold = 100;
        private readonly object _lock = new();

        public int Gold { get { lock (_lock) return _gold; } }
        public bool TrySpend(int amount) { lock (_lock) { if (_gold < amount) return false; _gold -= amount; return true; } }
        public void AddGold(int amount) { lock (_lock) { _gold += amount; } }
        public void SetGold(int amount) { lock (_lock) { _gold = amount; } }
    }
}
