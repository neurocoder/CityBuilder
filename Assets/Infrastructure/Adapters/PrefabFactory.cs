using System.Collections.Generic;
using UnityEngine;
using CityBuilder.Domain.Entities;

namespace CityBuilder.Infrastructure.Adapters
{
    public class PrefabFactory : MonoBehaviour
    {
        [System.Serializable] public struct Entry { public BuildingType Type; public GameObject Prefab; }
        [SerializeField] private Entry[] _entries = new Entry[0];
        private Dictionary<BuildingType, GameObject> _map = new();

        private void Awake()
        {
            _map = new Dictionary<BuildingType, GameObject>(_entries.Length);
            foreach (var e in _entries) if (e.Prefab != null) _map[e.Type] = e.Prefab;
        }

        public GameObject? Create(BuildingType type) => _map.TryGetValue(type, out var prefab) ? Instantiate(prefab) : null;
    }
}
