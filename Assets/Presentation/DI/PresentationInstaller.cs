using UnityEngine;
using VContainer;
using VContainer.Unity;
using CityBuilder.Presentation.UI;
using CityBuilder.Presentation.Presenters;
using CityBuilder.Presentation.Input;

namespace CityBuilder.Presentation.DI
{
    public class PresentationInstaller : LifetimeScope
    {
        [SerializeField] private HudPresenter hudPresenter = null!;
        [SerializeField] private BuildingPresenter buildingPresenter = null!;
        [SerializeField] private InputController inputController = null!;
        [SerializeField] private CameraController cameraController = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            if (hudPresenter != null)
                builder.RegisterComponent(hudPresenter);

            if (buildingPresenter != null)
                builder.RegisterComponent(buildingPresenter);

            if (inputController != null)
                builder.RegisterComponent(inputController);

            if (cameraController != null)
                builder.RegisterComponent(cameraController);
        }
    }
}
