using System.Collections.Generic;
using UnityEngine;
using CityBuilder.Application.DTOs;
using CityBuilder.Application.Interfaces;

namespace CityBuilder.Infrastructure.Adapters
{
    public class JsonSaveAdapter : MonoBehaviour, ISaveAdapter
    {
        private const string SaveKey = "MiniCity_Save_v1";

        [System.Serializable]
        private class Wrapper
        {
            public List<BuildingDTO> Buildings = new();
        }

        public void Save(List<BuildingDTO> buildings)
        {
            var wrapper = new Wrapper { Buildings = buildings };
            var json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
            Debug.Log("Saved: " + json);
        }

        public List<BuildingDTO> Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
                return new List<BuildingDTO>();

            var json = PlayerPrefs.GetString(SaveKey);
            var wrapper = JsonUtility.FromJson<Wrapper>(json);
            return wrapper?.Buildings ?? new List<BuildingDTO>();
        }
    }
}
