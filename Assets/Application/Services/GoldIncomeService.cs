using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using CityBuilder.Application.Interfaces;
using CityBuilder.Application.Events;

namespace CityBuilder.Application.Services
{
    public class GoldIncomeService : IDisposable
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly IEventBus _eventBus;
        private readonly int _incomeIntervalMs;
        private readonly int _incomeAmount;

        private CancellationTokenSource? _cancellationTokenSource;
        private UniTask? _incomeTask;

        public GoldIncomeService(IResourceRepository resourceRepository, IEventBus eventBus)
        {
            _resourceRepository = resourceRepository;
            _eventBus = eventBus;
            _incomeIntervalMs = 2000;
            _incomeAmount = 1;
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _incomeTask = RunIncomeLoop(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async UniTask RunIncomeLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await EarnGoldAsync(cancellationToken);
                    await UniTask.Delay(_incomeIntervalMs, cancellationToken: cancellationToken);
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

        private async UniTask EarnGoldAsync(CancellationToken cancellationToken)
        {
            await _resourceRepository.AddGoldAsync(_incomeAmount, cancellationToken);
            _eventBus.Publish(new GoldEarnedEvent(_incomeAmount));
        }

        public void Dispose() => Stop();
    }
}
