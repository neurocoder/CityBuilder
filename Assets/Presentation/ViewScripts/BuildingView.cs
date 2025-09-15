using System;
using UnityEngine;

namespace CityBuilder.Presentation.ViewScripts
{
    public class BuildingView : MonoBehaviour
    {
        [SerializeField] private GameObject _selectionIndicator = null!;
        
        public Guid BuildingId { get; private set; }
        
        public void Init(Guid id) 
        { 
            BuildingId = id;
            SetSelected(false);
        }
        
        public void UpdateLevel(int level) => transform.localScale = Vector3.one * (1f + 0.15f * (level - 1));
        
        public void SetSelected(bool isSelected)
        {
            if (_selectionIndicator != null)
                _selectionIndicator.SetActive(isSelected);
        }
    }
}
