using System;
using VContainer.Unity;
using CityBuilder.Application.Services;

namespace CityBuilder.Infrastructure.DI
{
    public sealed class StartupEntryPoint : IInitializable, IDisposable
    {
        private readonly EconomyService _economy;
        private readonly AutoSaveService? _autoSave;
        private readonly GoldIncomeService? _goldIncome;

        public StartupEntryPoint(EconomyService economy, AutoSaveService? autoSave = null, GoldIncomeService? goldIncome = null)
        {
            _economy = economy;
            _autoSave = autoSave;
            _goldIncome = goldIncome;
        }

        public void Initialize()
        {
            _economy?.Start();
            _autoSave?.Start();
            _goldIncome?.Start();
        }

        public void Dispose()
        {
            _economy?.Stop();
            _autoSave?.Stop();
            _goldIncome?.Stop();
        }
    }
}
