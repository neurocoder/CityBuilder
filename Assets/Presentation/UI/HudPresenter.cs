using System;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using R3;
using CityBuilder.Application.Interfaces;
using CityBuilder.Application.Services;
using CityBuilder.Application.Events;
using CityBuilder.Domain.Entities;
using CityBuilder.Application.UseCases;

namespace CityBuilder.Presentation.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class HudPresenter : MonoBehaviour
    {
        private IResourceRepository? _resources;
        private SaveLoadService? _saveLoad;
        private IEventBus? _events;
        private IBuildingRepository? _repo;
        private PlaceBuildingUseCase? _placeUseCase;
        private MoveBuildingUseCase? _moveUseCase;
        private RemoveBuildingUseCase? _removeUseCase;
        private UpgradeBuildingUseCase? _upgradeUseCase;
        private Presenters.BuildingPresenter? _buildingPresenter;
        
        private UIDocument? _uiDocument;
        private Label? _goldLabel;
        private Button? _saveButton;
        private Button? _loadButton;
        private Button? _houseButton;
        private Button? _farmButton;
        private Button? _mineButton;
        private Label? _selectedBuildingLabel;
        private Button? _moveButton;
        private Button? _upgradeButton;
        private Button? _deleteButton;
        private Label? _notificationLabel;

        private BuildingType? _selectedType;
        private Guid? _selectedBuildingId;
        private bool _isMoveMode;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Construct(IResourceRepository resources, SaveLoadService saveLoad, IEventBus events, IBuildingRepository repo, 
            PlaceBuildingUseCase placeUseCase, MoveBuildingUseCase moveUseCase, RemoveBuildingUseCase removeUseCase, UpgradeBuildingUseCase upgradeUseCase, CityBuilder.Presentation.Presenters.BuildingPresenter buildingPresenter)
        { 
            _resources = resources; 
            _saveLoad = saveLoad; 
            _events = events;
            _repo = repo;
            _placeUseCase = placeUseCase;
            _moveUseCase = moveUseCase;
            _removeUseCase = removeUseCase;
            _upgradeUseCase = upgradeUseCase;
            _buildingPresenter = buildingPresenter;
        }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var root = _uiDocument.rootVisualElement;
            
            _goldLabel = root.Q<Label>("goldLabel");
            _saveButton = root.Q<Button>("saveButton");
            _loadButton = root.Q<Button>("loadButton");
            _houseButton = root.Q<Button>("houseButton");
            _farmButton = root.Q<Button>("farmButton");
            _mineButton = root.Q<Button>("mineButton");
            _selectedBuildingLabel = root.Q<Label>("selectedBuildingLabel");
            _moveButton = root.Q<Button>("moveButton");
            _upgradeButton = root.Q<Button>("upgradeButton");
            _deleteButton = root.Q<Button>("deleteButton");
            _notificationLabel = root.Q<Label>("notificationLabel");

            _saveButton?.RegisterCallback<ClickEvent>(evt => SaveGameAsync());
            _loadButton?.RegisterCallback<ClickEvent>(evt => LoadGameAsync());
            _houseButton?.RegisterCallback<ClickEvent>(evt => SelectBuildingType(BuildingType.House));
            _farmButton?.RegisterCallback<ClickEvent>(evt => SelectBuildingType(BuildingType.Farm));
            _mineButton?.RegisterCallback<ClickEvent>(evt => SelectBuildingType(BuildingType.Mine));
            _moveButton?.RegisterCallback<ClickEvent>(evt => ToggleMoveMode());
            _upgradeButton?.RegisterCallback<ClickEvent>(evt => UpgradeSelectedBuilding());
            _deleteButton?.RegisterCallback<ClickEvent>(evt => DeleteSelectedBuilding());

            if (_resources != null)
            {
                _resources.GoldObservable
                    .Subscribe(gold => UpdateGoldDisplay(gold))
                    .AddTo(_disposables);
            }

            _events?.Subscribe<BuildingPlacedEvent>(OnBuildingPlaced);
            _events?.Subscribe<BuildingRemovedEvent>(OnBuildingRemoved);
            _events?.Subscribe<BuildingMovedEvent>(OnBuildingMoved);
            _events?.Subscribe<BuildingUpgradedEvent>(OnBuildingUpgraded);
            _events?.Subscribe<NotEnoughGoldEvent>(OnNotEnoughGold);
            _events?.Subscribe<CellOccupiedEvent>(OnCellOccupied);
            _events?.Subscribe<GameSavedEvent>(OnGameSaved);
            _events?.Subscribe<GameLoadedEvent>(OnGameLoaded);
        }

        private void UpdateGoldDisplay(int gold)
        {
            if (_goldLabel != null)
                _goldLabel.text = $"Gold: {gold}";
        }

        public void SelectBuildingType(BuildingType type)
        {
            _selectedType = type;
            _isMoveMode = false;
            _selectedBuildingId = null;
            _buildingPresenter?.SetSelectedBuilding(null);
            UpdateUI();
            ShowNotification($"Selected {type} for building");
        }

        private void ToggleMoveMode()
        {
            if (_selectedBuildingId.HasValue)
            {
                _isMoveMode = !_isMoveMode;
                _selectedType = null;
                if (!_isMoveMode)
                {
                    _selectedBuildingId = null;
                    _buildingPresenter?.SetSelectedBuilding(null);
                }
                UpdateUI();
                ShowNotification(_isMoveMode ? "Move mode ON - Click on empty cell to move building" : "Move mode OFF");
            }
            else
            {
                ShowNotification("Select a building first to enter move mode");
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
                ShowNotification("Select a building first to upgrade");
            }
        }

        private async void DeleteSelectedBuilding()
        {
            if (_selectedBuildingId.HasValue && _removeUseCase != null)
            {
                await _removeUseCase.ExecuteAsync(_selectedBuildingId.Value);
                _selectedBuildingId = null;
                _buildingPresenter?.SetSelectedBuilding(null);
            }
            else
            {
                ShowNotification("Select a building first to delete");
            }
        }

        private void UpdateUI()
        {
            UpdateButtonState(_houseButton, _selectedType == BuildingType.House);
            UpdateButtonState(_farmButton, _selectedType == BuildingType.Farm);
            UpdateButtonState(_mineButton, _selectedType == BuildingType.Mine);

            if (_selectedBuildingId.HasValue && _repo != null)
            {
                var building = _repo.FindById(_selectedBuildingId.Value);
                if (building != null)
                {
                    _selectedBuildingLabel.text = $"Selected: {building.Type} Level {building.Level.Level}";
                }
            }
            else
            {
                _selectedBuildingLabel.text = "No building selected";
            }

            _moveButton.SetEnabled(_selectedBuildingId.HasValue);
            _upgradeButton.SetEnabled(_selectedBuildingId.HasValue);
            _deleteButton.SetEnabled(_selectedBuildingId.HasValue);
        }

        private void UpdateButtonState(Button? button, bool isSelected)
        {
            if (button != null)
            {
                button.style.backgroundColor = isSelected ? 
                    new StyleColor(new Color(0, 0.8f, 0, 0.8f)) : 
                    new StyleColor(new Color(0, 0.4f, 0, 0.8f));
            }
        }

        private void ShowNotification(string message)
        {
            if (_notificationLabel != null)
            {
                _notificationLabel.text = message;
                Invoke(nameof(ClearNotification), 3f);
            }
        }

        private void ClearNotification()
        {
            if (_notificationLabel != null)
                _notificationLabel.text = "";
        }

        private void OnBuildingPlaced(BuildingPlacedEvent e)
        {
            ShowNotification($"{e.Building.Type} placed successfully!");
        }

        public void SetSelectedBuilding(Guid? buildingId)
        {
            _selectedBuildingId = buildingId;
            _buildingPresenter?.SetSelectedBuilding(buildingId);
            UpdateUI();
        }

        private void OnBuildingRemoved(BuildingRemovedEvent e)
        {
            _selectedBuildingId = null;
            _isMoveMode = false;
            _buildingPresenter?.SetSelectedBuilding(null);
            UpdateUI();
            ShowNotification("Building removed");
        }

        private void OnBuildingMoved(BuildingMovedEvent e)
        {
            ShowNotification($"{e.Building.Type} moved successfully!");
        }

        private void OnBuildingUpgraded(BuildingUpgradedEvent e)
        {
            ShowNotification($"{e.Building.Type} upgraded to level {e.Building.Level.Level}!");
        }

        private void OnNotEnoughGold(NotEnoughGoldEvent e)
        {
            ShowNotification($"Not enough gold! Need {e.Required} gold.");
        }

        private void OnCellOccupied(CellOccupiedEvent e)
        {
            ShowNotification($"Cell ({e.X}, {e.Y}) is occupied!");
        }

        private void OnGameSaved(GameSavedEvent e)
        {
            ShowNotification("Game saved successfully!");
        }

        private void OnGameLoaded(GameLoadedEvent e)
        {
            ShowNotification($"Game loaded! {e.Buildings.Count} buildings restored.");
        }

        private async void SaveGameAsync()
        {
            if (_saveButton != null)
            {
                _saveButton.SetEnabled(false);
                _saveButton.text = "Saving...";
            }

            try
            {
                if (_saveLoad != null)
                {
                    await _saveLoad.SaveAsync();
                }
            }
            catch (System.Exception ex)
            {
                ShowNotification($"Save failed: {ex.Message}");
            }
            finally
            {
                if (_saveButton != null)
                {
                    _saveButton.SetEnabled(true);
                    _saveButton.text = "Save";
                }
            }
        }

        private async void LoadGameAsync()
        {
            if (_loadButton != null)
            {
                _loadButton.SetEnabled(false);
                _loadButton.text = "Loading...";
            }

            try
            {
                if (_saveLoad != null)
                {
                    await _saveLoad.LoadAsync();
                }
            }
            catch (System.Exception ex)
            {
                ShowNotification($"Load failed: {ex.Message}");
            }
            finally
            {
                if (_loadButton != null)
                {
                    _loadButton.SetEnabled(true);
                    _loadButton.text = "Load";
                }
            }
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}
