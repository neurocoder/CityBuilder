using System;
using UnityEngine;

namespace CityBuilder.Presentation.ViewScripts
{
    public class BuildingView : MonoBehaviour
    {
        public Guid BuildingId { get; private set; }
        public void Init(Guid id) => BuildingId = id;
        public void UpdateLevel(int level) => transform.localScale = Vector3.one * (1f + 0.15f * (level - 1));
    }
}
