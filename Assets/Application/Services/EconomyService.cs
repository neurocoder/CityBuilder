using System;
using System.Threading;
using CityBuilder.Application.Interfaces;

namespace CityBuilder.Application.Services
{
    public class EconomyService : IDisposable
    {
        private readonly IBuildingRepository _repo;
        private readonly IResourceRepository _resources;
        private readonly int _tickMs;
        private Timer? _timer;

        public EconomyService(IBuildingRepository repo, IResourceRepository resources)
        {
            _repo = repo; _resources = resources; _tickMs = 1000;
        }

        public void Start() => _timer = new Timer(_ => Tick(), null, _tickMs, _tickMs);
        public void Stop() { _timer?.Change(Timeout.Infinite, Timeout.Infinite); _timer?.Dispose(); _timer = null; }

        private void Tick() { try { int total=0; foreach(var b in _repo.GetAll()) total += b.Level.IncomePerTick; if(total>0) _resources.AddGold(total); } catch { } }
        public void Dispose() => Stop();
    }
}
