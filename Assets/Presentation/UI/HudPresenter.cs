using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using CityBuilder.Application.Interfaces;
using CityBuilder.Application.Services;

namespace CityBuilder.Presentation.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class HudPresenter : MonoBehaviour
    {
        private IResourceRepository? _resources;
        private SaveLoadService? _saveLoad;
        private UIDocument? _uiDocument;
        private Label? _goldLabel;
        private Button? _saveButton;
        private Button? _loadButton;

        [Inject]
        public void Construct(IResourceRepository resources, SaveLoadService saveLoad) { _resources = resources; _saveLoad = saveLoad; }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var root = _uiDocument.rootVisualElement;
            _goldLabel = root.Q<Label>("goldLabel");
            _saveButton = root.Q<Button>("saveButton");
            _loadButton = root.Q<Button>("loadButton");
            _saveButton?.RegisterCallback<ClickEvent>(evt => _saveLoad?.Save());
            _loadButton?.RegisterCallback<ClickEvent>(evt => _saveLoad?.Load());
        }

        private void Update() { if (_goldLabel != null && _resources != null) _goldLabel.text = $"Gold: {_resources.Gold}"; }
    }
}
