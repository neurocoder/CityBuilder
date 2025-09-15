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
        private CityBuilder.Presentation.UI.HudPresenter? _hudPresenter;

        private BuildingType? _selectedType;
        private Guid? _selectedBuildingId;
        private bool _isMoveMode;

        [Inject]
        public void Construct(PlaceBuildingUseCase place, MoveBuildingUseCase move, RemoveBuildingUseCase remove, UpgradeBuildingUseCase upgrade, IBuildingRepository repo, IEventBus events, CityBuilder.Presentation.UI.HudPresenter hudPresenter)
        {
            _placeUseCase = place; _moveUseCase = move; _removeUseCase = remove; _upgradeUseCase = upgrade; _repo = repo; _events = events; _hudPresenter = hudPresenter;
        }

        private void Update()
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectType(BuildingType.House);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectType(BuildingType.Farm);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectType(BuildingType.Mine);
            if (Keyboard.current.mKey.wasPressedThisFrame) ToggleMoveMode();
            if (Keyboard.current.uKey.wasPressedThisFrame) UpgradeSelectedBuilding();
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

        private void SelectType(BuildingType type) 
        { 
            _selectedType = type; 
            _isMoveMode = false; 
            _selectedBuildingId = null; 
            _hudPresenter?.SelectBuildingType(type);
        }
        private void HandleLeftClick(int x, int y)
        {
            var pos = new GridPosition(x, y);
            var b = _repo?.FindByPosition(pos);
            
            if (b != null)
            {
                if (_isMoveMode && _selectedBuildingId.HasValue)
                {
                    // Move selected building to new position
                    _moveUseCase?.Execute(_selectedBuildingId.Value, pos);
                    _isMoveMode = false;
                    _selectedBuildingId = null;
                }
                else
                {
                    // Select building
                    _selectedBuildingId = b.Id;
                    _hudPresenter?.SetSelectedBuilding(b.Id);
                }
                return;
            }
            
            if (_selectedType.HasValue) _placeUseCase?.Execute(_selectedType.Value, pos);
        }

        private void ToggleMoveMode()
        {
            if (_selectedBuildingId.HasValue)
            {
                _isMoveMode = !_isMoveMode;
                _selectedType = null; // Clear building type selection
                Debug.Log(_isMoveMode ? "Move mode ON - Click on empty cell to move building" : "Move mode OFF");
            }
            else
            {
                Debug.Log("Select a building first to enter move mode");
            }
        }

        private void UpgradeSelectedBuilding()
        {
            if (_selectedBuildingId.HasValue)
            {
                _upgradeUseCase?.Execute(_selectedBuildingId.Value);
            }
            else
            {
                Debug.Log("Select a building first to upgrade");
            }
        }
    }
}
