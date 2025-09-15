using R3;
using Cysharp.Threading.Tasks;
using System.Threading;
using CityBuilder.Application.Interfaces;

namespace CityBuilder.Infrastructure.Adapters
{
    public class InMemoryResourceRepository : IResourceRepository
    {
        private readonly ReactiveProperty<int> _gold = new(100);
        private readonly object _lock = new();

        public int Gold => _gold.Value;
        public ReadOnlyReactiveProperty<int> GoldObservable => _gold;

        public async UniTask<bool> TrySpendAsync(int amount, CancellationToken cancellationToken = default)
        {
            await UniTask.SwitchToMainThread(cancellationToken);

            lock (_lock)
            {
                if (_gold.Value < amount) return false;
                _gold.Value -= amount;
                return true;
            }
        }

        public async UniTask AddGoldAsync(int amount, CancellationToken cancellationToken = default)
        {
            await UniTask.SwitchToMainThread(cancellationToken);

            lock (_lock)
            {
                _gold.Value += amount;
            }
        }

        public async UniTask SetGoldAsync(int amount, CancellationToken cancellationToken = default)
        {
            await UniTask.SwitchToMainThread(cancellationToken);

            lock (_lock)
            {
                _gold.Value = amount;
            }
        }
    }
}