using System;
using UnityEngine;
using CityBuilder.Domain.Entities;
using CityBuilder.Domain.Rules;
using CityBuilder.Application.Interfaces;
using VContainer;

namespace CityBuilder.Presentation.Input
{
    public class BuildingPreviewController : MonoBehaviour
    {
        [SerializeField] private GameObject _previewPrefab = null!;
        [SerializeField] private SpriteRenderer _previewRenderer = null!;
        [SerializeField] private Color _validColor = Color.green;
        [SerializeField] private Color _invalidColor = Color.red;
        [SerializeField] private float _previewAlpha = 0.5f;
        [SerializeField] private int _sortingOrder = 100;

        private IGridService? _grid;
        private IResourceRepository? _resources;
        private BuildingType? _selectedType;
        private GameObject? _currentPreview;
        private bool _isPreviewActive;
        private bool _isMoveMode;

        [Inject]
        public void Construct(IGridService grid, IResourceRepository resources)
        {
            _grid = grid;
            _resources = resources;
        }

        private void Start()
        {
            if (_previewPrefab != null)
            {
                _currentPreview = Instantiate(_previewPrefab);
                _currentPreview.SetActive(false);
                _previewRenderer = _currentPreview.GetComponent<SpriteRenderer>();
                
                if (_previewRenderer != null)
                {
                    _previewRenderer.sortingOrder = _sortingOrder;
                }
            }
        }

        private void OnDestroy()
        {
            if (_currentPreview != null)
            {
                Destroy(_currentPreview);
            }
        }

        public void SetSelectedBuildingType(BuildingType? type)
        {
            _selectedType = type;
            _isMoveMode = false;
            if (type == null)
            {
                HidePreview();
            }
        }

        public void SetMoveMode(bool isMoveMode)
        {
            _isMoveMode = isMoveMode;
            if (!isMoveMode)
            {
                HidePreview();
            }
        }

        public void UpdatePreview(Vector3 worldPosition)
        {
            if ((_selectedType == null && !_isMoveMode) || _currentPreview == null || _previewRenderer == null)
            {
                HidePreview();
                return;
            }

            var cellX = Mathf.FloorToInt(worldPosition.x);
            var cellY = Mathf.FloorToInt(worldPosition.y);
            var gridPos = new GridPosition(cellX, cellY);

            _currentPreview.SetActive(true);
            _currentPreview.transform.position = new Vector3(cellX + 0.5f, cellY + 0.5f, 0f);

            bool isCellFree = _grid?.IsCellFree(gridPos) ?? false;
            bool hasEnoughResources = _isMoveMode || HasEnoughResources();

            var color = (isCellFree && hasEnoughResources) ? _validColor : _invalidColor;
            color.a = _previewAlpha;
            _previewRenderer.color = color;

            _isPreviewActive = true;
        }

        public void HidePreview()
        {
            if (_currentPreview != null)
            {
                _currentPreview.SetActive(false);
            }
            _isPreviewActive = false;
        }

        private bool HasEnoughResources()
        {
            if (_selectedType == null || _resources == null)
                return false;

            var cost = GetBuildingCost(_selectedType.Value);
            return _resources.Gold >= cost;
        }

        private int GetBuildingCost(BuildingType type)
        {
            var defaultLevel = UpgradeRules.GetDefaultLevel(type);
            return defaultLevel.Cost;
        }

        public bool IsPreviewActive => _isPreviewActive;
    }
}
