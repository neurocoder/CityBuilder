using System;
using UnityEngine;
using UnityEngine.InputSystem;
using CityBuilder.Domain.Entities;
using CityBuilder.Application.UseCases;
using CityBuilder.Application.Interfaces;
using VContainer;
using CityBuilder.Application.Events;

namespace CityBuilder.Presentation.Input
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera = null!;
        [SerializeField] private BuildingPreviewController _previewController = null!;

        private PlaceBuildingUseCase? _placeUseCase;
        private MoveBuildingUseCase? _moveUseCase;
        private RemoveBuildingUseCase? _removeUseCase;
        private UpgradeBuildingUseCase? _upgradeUseCase;
        private IBuildingRepository? _repo;
        private IEventBus? _events;
        private Presenters.BuildingPresenter? _buildingPresenter;

        private BuildingType? _selectedType;
        private Guid? _selectedBuildingId;
        private bool _isMoveMode;

        [Inject]
        public void Construct(PlaceBuildingUseCase place, MoveBuildingUseCase move, RemoveBuildingUseCase remove, UpgradeBuildingUseCase upgrade, IBuildingRepository repo, IEventBus events, CityBuilder.Presentation.Presenters.BuildingPresenter buildingPresenter)
        {
            _placeUseCase = place; _moveUseCase = move; _removeUseCase = remove; _upgradeUseCase = upgrade; _repo = repo; _events = events; _buildingPresenter = buildingPresenter;
            
            _events?.Subscribe<BuildingTypeSelectedEvent>(OnBuildingTypeSelected);
            _events?.Subscribe<MoveModeToggledEvent>(OnMoveModeToggled);
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectType(BuildingType.House);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectType(BuildingType.Farm);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectType(BuildingType.Mine);
            if (Keyboard.current.mKey.wasPressedThisFrame) ToggleMoveMode();
            if (Keyboard.current.uKey.wasPressedThisFrame) UpgradeSelectedBuilding();
            if (Keyboard.current.deleteKey.wasPressedThisFrame) { if (_selectedBuildingId.HasValue) RemoveSelectedBuildingAsync(); }

            var ms = Mouse.current;
            if (ms == null) return;
            var mp = ms.position.ReadValue();
            var world = _mainCamera.ScreenToWorldPoint(new Vector3(mp.x, mp.y, -_mainCamera.transform.position.z));
            var cellX = Mathf.FloorToInt(world.x);
            var cellY = Mathf.FloorToInt(world.y);

            if ((_selectedType.HasValue && !_isMoveMode) || (_isMoveMode && _selectedBuildingId.HasValue))
            {
                _previewController?.UpdatePreview(world);
            }
            else
            {
                _previewController?.HidePreview();
            }

            if (ms.leftButton.wasPressedThisFrame) HandleLeftClickAsync(cellX, cellY);
        }

        public void SelectType(BuildingType type)
        {
            _selectedType = type;
            _isMoveMode = false;
            _selectedBuildingId = null;
            _buildingPresenter?.SetSelectedBuilding(null);
            _previewController?.SetSelectedBuildingType(type);
            _events?.Publish(new BuildingTypeSelectedEvent(type));
        }
        private async void HandleLeftClickAsync(int x, int y)
        {
            var pos = new GridPosition(x, y);
            var building = _repo?.FindByPosition(pos);

            if (_isMoveMode && _selectedBuildingId.HasValue)
            {
                if (building == null)
                {
                    if (_moveUseCase != null)
                    {
                        await _moveUseCase.ExecuteAsync(_selectedBuildingId.Value, pos);
                    }
                    _isMoveMode = false;
                    _selectedBuildingId = null;
                    Debug.Log($"Moved building to position ({x}, {y})");
                }
                else
                {
                    Debug.Log("Cannot move building to occupied cell");
                }
                return;
            }

            if (building != null)
            {
                _selectedBuildingId = building.Id;
                _buildingPresenter?.SetSelectedBuilding(building.Id);
                Debug.Log($"Selected building at position ({x}, {y})");
                return;
            }

            if (_selectedType.HasValue && _placeUseCase != null)
            {
                await _placeUseCase.ExecuteAsync(_selectedType.Value, pos);
                Debug.Log($"Placed {_selectedType.Value} at position ({x}, {y})");
            }
        }

        public void ToggleMoveMode()
        {
            if (_selectedBuildingId.HasValue)
            {
                _isMoveMode = !_isMoveMode;
                _selectedType = null;

                if (_isMoveMode)
                {
                    _previewController?.SetMoveMode(true);
                }
                else
                {
                    _previewController?.SetMoveMode(false);
                    _selectedBuildingId = null;
                    _buildingPresenter?.SetSelectedBuilding(null);
                }
                _events?.Publish(new MoveModeToggledEvent(_isMoveMode));
                Debug.Log(_isMoveMode ? "Move mode ON - Click on empty cell to move building" : "Move mode OFF");
            }
            else
            {
                Debug.Log("Select a building first to enter move mode");
            }
        }

        private async void UpgradeSelectedBuilding()
        {
            if (_selectedBuildingId.HasValue && _upgradeUseCase != null)
            {
                await _upgradeUseCase.ExecuteAsync(_selectedBuildingId.Value);
            }
            else
            {
                Debug.Log("Select a building first to upgrade");
            }
        }

        private async void RemoveSelectedBuildingAsync()
        {
            if (_selectedBuildingId.HasValue && _removeUseCase != null)
            {
                await _removeUseCase.ExecuteAsync(_selectedBuildingId.Value);
                _selectedBuildingId = null;
                _buildingPresenter?.SetSelectedBuilding(null);
            }
        }

        private void OnBuildingTypeSelected(BuildingTypeSelectedEvent e)
        {
            _selectedType = e.Type;
            _isMoveMode = false;
            _selectedBuildingId = null;
            _buildingPresenter?.SetSelectedBuilding(null);
            _previewController?.SetSelectedBuildingType(e.Type);
        }

        private void OnMoveModeToggled(MoveModeToggledEvent e)
        {
            _isMoveMode = e.IsMoveMode;
            _selectedType = null;
            
            if (_isMoveMode)
            {
                _previewController?.SetMoveMode(true);
            }
            else
            {
                _previewController?.SetMoveMode(false);
                _selectedBuildingId = null;
                _buildingPresenter?.SetSelectedBuilding(null);
            }
        }
    }
}
