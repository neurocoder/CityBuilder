using UnityEngine;

namespace CityBuilder.Presentation.Input
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Move Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float dragSpeed = 2f;

        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minZoom = 2f;
        [SerializeField] private float maxZoom = 20f;

        private Camera _camera;
        private Vector3 _dragOrigin;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            if (!_camera.orthographic)
            {
                Debug.LogWarning("CameraController: expected orthographic camera, switching.");
                _camera.orthographic = true;
            }
        }

        private void Update()
        {
            HandleKeyboardMove();
            HandleMouseDrag();
            HandleZoom();
        }

        private void HandleKeyboardMove()
        {
            float h = UnityEngine.Input.GetAxis("Horizontal");
            float v = UnityEngine.Input.GetAxis("Vertical");
            Vector3 dir = new(h, v, 0f);
            transform.position += dir * (moveSpeed * Time.deltaTime);
        }

        private void HandleMouseDrag()
        {
            if (UnityEngine.Input.GetMouseButtonDown(2))
                _dragOrigin = UnityEngine.Input.mousePosition;

            if (!UnityEngine.Input.GetMouseButton(2)) return;

            Vector3 pos = _camera.ScreenToViewportPoint(UnityEngine.Input.mousePosition - _dragOrigin);
            Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);
            transform.Translate(move, Space.World);
        }

        private void HandleZoom()
        {
            float scroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                float size = _camera.orthographicSize - scroll * zoomSpeed;
                _camera.orthographicSize = Mathf.Clamp(size, minZoom, maxZoom);
            }
        }
    }
}
