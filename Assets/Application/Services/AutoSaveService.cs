using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CityBuilder.Application.Services
{
    public sealed class AutoSaveService : IDisposable
    {
        private readonly SaveLoadService _saveLoad;
        private readonly int _intervalMs;
        private CancellationTokenSource? _cts;

        public AutoSaveService(SaveLoadService saveLoad)
        {
            _saveLoad = saveLoad;
            _intervalMs = Math.Max(1000, 30 * 1000); // 30 seconds default
        }

        public void Start()
        {
            if (_cts != null) return;
            _cts = new CancellationTokenSource();
            _ = LoopAsync(_cts.Token);
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts = null;
        }

        private async UniTaskVoid LoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await UniTask.Delay(_intervalMs, cancellationToken: token);
                    _saveLoad.Save();
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Dispose() => Stop();
    }
}
