using System;
using UnityEngine;
using UnityEngine.InputSystem;
using CityBuilder.Domain.Entities;
using CityBuilder.Application.UseCases;
using CityBuilder.Application.Interfaces;
using VContainer;

namespace CityBuilder.Presentation.Input
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera = null!;
        [SerializeField] private GameObject _tileSelector = null!;

        private PlaceBuildingUseCase? _placeUseCase;
        private MoveBuildingUseCase? _moveUseCase;
        private RemoveBuildingUseCase? _removeUseCase;
        private UpgradeBuildingUseCase? _upgradeUseCase;
        private IBuildingRepository? _repo;
        private IEventBus? _events;

        private BuildingType? _selectedType;
        private Guid? _selectedBuildingId;
        private bool _isMoveMode;

        [Inject]
        public void Construct(PlaceBuildingUseCase place, MoveBuildingUseCase move, RemoveBuildingUseCase remove, UpgradeBuildingUseCase upgrade, IBuildingRepository repo, IEventBus events)
        {
            _placeUseCase = place; _moveUseCase = move; _removeUseCase = remove; _upgradeUseCase = upgrade; _repo = repo; _events = events;
        }

        private void Update()
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectType(BuildingType.House);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectType(BuildingType.Farm);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectType(BuildingType.Mine);
            if (Keyboard.current.rKey.wasPressedThisFrame) _ = true; // placeholder
            if (Keyboard.current.deleteKey.wasPressedThisFrame) { if (_selectedBuildingId.HasValue) _removeUseCase?.Execute(_selectedBuildingId.Value); }

            var ms = Mouse.current;
            if (ms == null) return;
            var mp = ms.position.ReadValue();
            var world = _mainCamera.ScreenToWorldPoint(new Vector3(mp.x, mp.y, -_mainCamera.transform.position.z));
            var cellX = Mathf.FloorToInt(world.x);
            var cellY = Mathf.FloorToInt(world.y);
            if (_tileSelector != null) _tileSelector.transform.position = new Vector3(cellX + 0.5f, cellY + 0.5f, 0f);

            if (ms.leftButton.wasPressedThisFrame) HandleLeftClick(cellX, cellY);
        }

        private void SelectType(BuildingType type) { _selectedType = type; _isMoveMode = false; _selectedBuildingId = null; }
        private void HandleLeftClick(int x, int y)
        {
            var pos = new GridPosition(x, y);
            var b = _repo?.FindByPosition(pos);
            if (b != null) { _selectedBuildingId = b.Id; return; }
            if (_selectedType.HasValue) _placeUseCase?.Execute(_selectedType.Value, pos);
        }
    }
}
