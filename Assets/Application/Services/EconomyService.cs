using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using CityBuilder.Application.Interfaces;

namespace CityBuilder.Application.Services
{
    public class EconomyService : IDisposable
    {
        private readonly IBuildingRepository _repo;
        private readonly IResourceRepository _resources;
        private readonly int _tickMs;
        private CancellationTokenSource? _cancellationTokenSource;
        private UniTask? _economyTask;

        public EconomyService(IBuildingRepository repo, IResourceRepository resources)
        {
            _repo = repo;
            _resources = resources;
            _tickMs = 1000;
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _economyTask = RunEconomyLoop(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async UniTask RunEconomyLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await TickAsync(cancellationToken);
                    await UniTask.Delay(_tickMs, cancellationToken: cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                }
            }
        }

        private async UniTask TickAsync(CancellationToken cancellationToken)
        {
            int total = 0;
            foreach (var building in _repo.GetAll())
            {
                total += building.Level.IncomePerTick;
            }

            if (total > 0)
            {
                await _resources.AddGoldAsync(total, cancellationToken);
            }
        }

        public void Dispose() => Stop();
    }
}