using UnityEngine;
using VContainer;
using VContainer.Unity;
using CityBuilder.Application.Services;
using CityBuilder.Infrastructure.Adapters;
using CityBuilder.Infrastructure.EventBus;
using CityBuilder.Infrastructure.Services;
using CityBuilder.Application.Interfaces;

namespace CityBuilder.Infrastructure.DI
{
    public class GameInstaller : LifetimeScope
    {
        [SerializeField] private PrefabFactory _prefabFactory = null!;
        [SerializeField] private JsonSaveAdapter _saveAdapter = null!;
        [SerializeField] private GoldIncomeServiceRunner _goldIncomeServiceRunner = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_prefabFactory != null)
                builder.RegisterComponent(_prefabFactory);

            if (_saveAdapter != null)
                builder.RegisterComponent(_saveAdapter).As<ISaveAdapter>();

            if (_goldIncomeServiceRunner != null)
                builder.RegisterComponent(_goldIncomeServiceRunner);

            builder.Register<InMemoryBuildingRepository>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<InMemoryResourceRepository>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<GridService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SimpleEventBus>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EconomyService>(Lifetime.Singleton).AsSelf();
            builder.Register<SaveLoadService>(Lifetime.Singleton).AsSelf();
            builder.Register<AutoSaveService>(Lifetime.Singleton).AsSelf();
            builder.Register<GoldIncomeService>(Lifetime.Singleton).AsSelf();

            builder.Register<CityBuilder.Application.UseCases.PlaceBuildingUseCase>(Lifetime.Scoped);
            builder.Register<CityBuilder.Application.UseCases.MoveBuildingUseCase>(Lifetime.Scoped);
            builder.Register<CityBuilder.Application.UseCases.RemoveBuildingUseCase>(Lifetime.Scoped);
            builder.Register<CityBuilder.Application.UseCases.UpgradeBuildingUseCase>(Lifetime.Scoped);

            builder.RegisterEntryPoint<StartupEntryPoint>();
        }
    }
}
