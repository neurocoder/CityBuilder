using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using CityBuilder.Application.DTOs;

namespace CityBuilder.Application.Interfaces
{
    public interface ISaveAdapter
    {
        UniTask SaveAsync(GameStateDTO gameState, CancellationToken cancellationToken = default);
        UniTask<GameStateDTO> LoadAsync(CancellationToken cancellationToken = default);
    }
}