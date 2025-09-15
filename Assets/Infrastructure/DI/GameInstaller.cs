// Assets/Infrastructure/DI/GameInstaller.cs
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

        protected override void Configure(IContainerBuilder builder)
        {
            // adapters
            if (_prefabFactory != null)
                builder.RegisterComponent(_prefabFactory);
            
            if (_saveAdapter != null)
                builder.RegisterComponent(_saveAdapter).As<ISaveAdapter>();

            // repositories
            builder.Register<InMemoryBuildingRepository>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<InMemoryResourceRepository>(Lifetime.Singleton).AsImplementedInterfaces();

            // services
            builder.Register<GridService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SimpleEventBus>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EconomyService>(Lifetime.Singleton).AsSelf();
            builder.Register<SaveLoadService>(Lifetime.Singleton).AsSelf();
            builder.Register<AutoSaveService>(Lifetime.Singleton).AsSelf();

            // use cases
            builder.Register<CityBuilder.Application.UseCases.PlaceBuildingUseCase>(Lifetime.Scoped);
            builder.Register<CityBuilder.Application.UseCases.MoveBuildingUseCase>(Lifetime.Scoped);
            builder.Register<CityBuilder.Application.UseCases.RemoveBuildingUseCase>(Lifetime.Scoped);
            builder.Register<CityBuilder.Application.UseCases.UpgradeBuildingUseCase>(Lifetime.Scoped);

            // startup entry
            builder.RegisterEntryPoint<StartupEntryPoint>();
        }
    }
}
