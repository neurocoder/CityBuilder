using System;
using VContainer;
using VContainer.Unity;
using CityBuilder.Application.Services;

namespace CityBuilder.Infrastructure.DI
{
    public sealed class StartupEntryPoint : IInitializable, IDisposable
    {
        private readonly EconomyService _economy;
        private readonly AutoSaveService? _autoSave;

        public StartupEntryPoint(EconomyService economy, AutoSaveService? autoSave = null)
        {
            _economy = economy;
            _autoSave = autoSave;
        }

        public void Initialize()
        {
            _economy?.Start();
            _autoSave?.Start();
        }

        public void Dispose()
        {
            _economy?.Stop();
            _autoSave?.Stop();
        }
    }
}
