using R3;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace CityBuilder.Application.Interfaces
{
    public interface IResourceRepository
    {
        int Gold { get; }
        ReadOnlyReactiveProperty<int> GoldObservable { get; }
        UniTask<bool> TrySpendAsync(int amount, CancellationToken cancellationToken = default);
        UniTask AddGoldAsync(int amount, CancellationToken cancellationToken = default);
        UniTask SetGoldAsync(int amount, CancellationToken cancellationToken = default);
    }
}