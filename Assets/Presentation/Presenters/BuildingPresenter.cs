using System;
using System.Collections.Generic;
using UnityEngine;
using CityBuilder.Application.Events;
using CityBuilder.Application.Interfaces;
using CityBuilder.Domain.Entities;
using CityBuilder.Infrastructure.Adapters;
using VContainer;

namespace CityBuilder.Presentation.Presenters
{
    [DisallowMultipleComponent]
    public class BuildingPresenter : MonoBehaviour
    {
        private IEventBus? _events;
        private IBuildingRepository? _repo;
        private PrefabFactory? _factory;
        private readonly Dictionary<Guid, GameObject> _views = new();
        private Guid? _selectedBuildingId;

        [Inject]
        public void Construct(IBuildingRepository repo, IEventBus events, PrefabFactory factory)
        {
            _repo = repo; _events = events; _factory = factory;
            _events.Subscribe<BuildingPlacedEvent>(OnPlaced);
            _events.Subscribe<BuildingRemovedEvent>(OnRemoved);
            _events.Subscribe<BuildingMovedEvent>(OnMoved);
            _events.Subscribe<BuildingUpgradedEvent>(OnUpgraded);
            _events.Subscribe<GameLoadedEvent>(OnLoaded);
        }

        public void SetSelectedBuilding(Guid? buildingId)
        {
            if (_selectedBuildingId.HasValue && _views.TryGetValue(_selectedBuildingId.Value, out var oldView))
            {
                var oldBuildingView = oldView.GetComponent<CityBuilder.Presentation.ViewScripts.BuildingView>();
                oldBuildingView?.SetSelected(false);
            }

            _selectedBuildingId = buildingId;

            if (buildingId.HasValue && _views.TryGetValue(buildingId.Value, out var newView))
            {
                var newBuildingView = newView.GetComponent<CityBuilder.Presentation.ViewScripts.BuildingView>();
                newBuildingView?.SetSelected(true);
            }
        }

        private void OnPlaced(BuildingPlacedEvent e)
        {
            var go = _factory?.Create(e.Building.Type);
            if (go == null) return;
            go.transform.position = new Vector3(e.Building.Position.X + 0.5f, e.Building.Position.Y + 0.5f, 0f);
            var view = go.GetComponent<CityBuilder.Presentation.ViewScripts.BuildingView>() ?? go.AddComponent<CityBuilder.Presentation.ViewScripts.BuildingView>();
            view.Init(e.Building.Id); view.UpdateLevel(e.Building.Level.Level);
            _views[e.Building.Id] = go;
        }

        private void OnRemoved(BuildingRemovedEvent e) 
        { 
            if (_views.TryGetValue(e.BuildingId, out var go)) Destroy(go); 
            _views.Remove(e.BuildingId);
            
            if (_selectedBuildingId == e.BuildingId)
            {
                _selectedBuildingId = null;
            }
        }
        private void OnMoved(BuildingMovedEvent e) { if (_views.TryGetValue(e.Building.Id, out var go)) go.transform.position = new Vector3(e.Building.Position.X + 0.5f, e.Building.Position.Y + 0.5f, 0f); }
        private void OnUpgraded(BuildingUpgradedEvent e) { if (_views.TryGetValue(e.Building.Id, out var go)) { var v = go.GetComponent<CityBuilder.Presentation.ViewScripts.BuildingView>(); v?.UpdateLevel(e.Building.Level.Level); } }

        private void OnLoaded(GameLoadedEvent e)
        {
            foreach (var kv in new List<KeyValuePair<Guid, GameObject>>(_views)) Destroy(kv.Value);
            _views.Clear();
            if (_factory == null) return;
            foreach (var b in e.Buildings)
            {
                var go = _factory.Create(b.Type);
                if (go == null) continue;
                go.transform.position = new Vector3(b.Position.X + 0.5f, b.Position.Y + 0.5f, 0f);
                var view = go.GetComponent<CityBuilder.Presentation.ViewScripts.BuildingView>() ?? go.AddComponent<CityBuilder.Presentation.ViewScripts.BuildingView>();
                view.Init(b.Id); view.UpdateLevel(b.Level.Level);
                _views[b.Id] = go;
            }
        }

        private void OnDestroy()
        {
            if (_events == null) return;
            _events.Unsubscribe<BuildingPlacedEvent>(OnPlaced);
            _events.Unsubscribe<BuildingRemovedEvent>(OnRemoved);
            _events.Unsubscribe<BuildingMovedEvent>(OnMoved);
            _events.Unsubscribe<BuildingUpgradedEvent>(OnUpgraded);
            _events.Unsubscribe<GameLoadedEvent>(OnLoaded);
        }
    }
}
