using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using CityBuilder.Application.DTOs;
using CityBuilder.Application.Interfaces;
using System;

namespace CityBuilder.Infrastructure.Adapters
{
    public class JsonSaveAdapter : MonoBehaviour, ISaveAdapter
    {
        private const string SaveKey = "MiniCity_Save_v1";

        [Serializable]
        private class Wrapper
        {
            public GameStateDTO GameState = new();
        }


        public async UniTask SaveAsync(GameStateDTO gameState, CancellationToken cancellationToken = default)
        {
            await UniTask.SwitchToMainThread(cancellationToken);

            try
            {
                var wrapper = new Wrapper { GameState = gameState };
                var json = JsonUtility.ToJson(wrapper);

                PlayerPrefs.SetString(SaveKey, json);
                PlayerPrefs.Save();
                Debug.Log("Saved: " + json);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to save: {ex.Message}");
                throw;
            }
        }

        public async UniTask<GameStateDTO> LoadAsync(CancellationToken cancellationToken = default)
        {
            await UniTask.SwitchToMainThread(cancellationToken);

            try
            {
                if (!PlayerPrefs.HasKey(SaveKey))
                    return new GameStateDTO();

                var json = PlayerPrefs.GetString(SaveKey);
                var wrapper = JsonUtility.FromJson<Wrapper>(json);
                return wrapper?.GameState ?? new GameStateDTO();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load: {ex.Message}");
                return new GameStateDTO();
            }
        }
    }
}