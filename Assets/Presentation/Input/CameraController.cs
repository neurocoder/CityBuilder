using UnityEngine;
using UnityEngine.InputSystem;

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
        private Vector2 _dragOrigin;
        private bool _isDragging;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            if (!_camera.orthographic)
            {
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
            Vector2 moveInput = Vector2.zero;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                    moveInput.y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                    moveInput.y -= 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                    moveInput.x -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                    moveInput.x += 1f;
            }

            if (moveInput.magnitude > 0)
            {
                Vector3 dir = new(moveInput.x, moveInput.y, 0f);
                transform.position += dir * (moveSpeed * Time.deltaTime);
            }
        }

        private void HandleMouseDrag()
        {
            if (Mouse.current == null) return;

            bool dragButtonPressed = Mouse.current.middleButton.isPressed || Mouse.current.rightButton.isPressed;

            if (Mouse.current.middleButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                _dragOrigin = Mouse.current.position.ReadValue();
                _isDragging = true;
                return;
            }

            if (!dragButtonPressed)
            {
                _isDragging = false;
                return;
            }

            if (!_isDragging) return;

            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            Vector2 delta = currentMousePos - _dragOrigin;

            if (delta.magnitude > 0.1f)
            {
                Vector3 worldDelta = _camera.ScreenToWorldPoint(new Vector3(delta.x, delta.y, _camera.nearClipPlane)) -
                                    _camera.ScreenToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));

                transform.Translate(-worldDelta * dragSpeed, Space.World);
                _dragOrigin = currentMousePos;
            }
        }

        private void HandleZoom()
        {
            if (Mouse.current == null) return;

            Vector2 scrollDelta = Mouse.current.scroll.ReadValue();
            float scroll = scrollDelta.y;

            if (Mathf.Abs(scroll) > 0.01f)
            {
                float size = _camera.orthographicSize - scroll * zoomSpeed * 0.01f;
                _camera.orthographicSize = Mathf.Clamp(size, minZoom, maxZoom);
            }
        }
    }
}
