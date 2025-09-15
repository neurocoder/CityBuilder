using UnityEngine;
using CityBuilder.Application.Interfaces;
using CityBuilder.Domain.Entities;
using VContainer;

namespace CityBuilder.Presentation.ViewScripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TileSelector : MonoBehaviour
    {
        [SerializeField] private Color freeColor = Color.green;
        [SerializeField] private Color occupiedColor = Color.red;

        private SpriteRenderer _renderer = null!;
        private IGridService? _gridService;

        [Inject]
        public void Construct(IGridService gridService)
        {
            _gridService = gridService;
        }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void UpdatePosition(GridPosition pos)
        {
            transform.position = new Vector3(pos.X + 0.5f, pos.Y + 0.5f, 0f);
            if (_gridService != null)
            {
                bool free = _gridService.IsCellFree(pos);
                _renderer.color = free ? freeColor : occupiedColor;
            }
        }
    }
}
