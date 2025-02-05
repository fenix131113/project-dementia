using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class CameraControl : MonoBehaviour
    {
        private Transform _playerCamera;
        private float _xRot;
        private float _startCamY;
        private Vector2 _lookInput;
        private Vector2 _currentLook;
        private Vector2 _lookVelocity;

        [SerializeField] private float sensitivity = 2f;
        [SerializeField] private float smoothTime = 0.1f;
        [SerializeField] private float shakeIntensity = 0.1f;
        [SerializeField] private PhotonView netView;

        private void Awake()
        {
            _playerCamera = transform.GetChild(0);
            _startCamY = _playerCamera.localPosition.y;
            HideCursor();
        }

        private void Update()
        {
            if (!netView.IsMine) return;
            SmoothCameraRotation();
        }

        public void OnMouseDelta(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>() * sensitivity;
        }

        private void SmoothCameraRotation()
        {
            _currentLook = Vector2.SmoothDamp(_currentLook, _lookInput, ref _lookVelocity, smoothTime);

            _xRot -= _currentLook.y;
            _xRot = Mathf.Clamp(_xRot, -70, 70);

            transform.Rotate(0, _currentLook.x, 0);
            _playerCamera.localRotation = Quaternion.Euler(_xRot + Random.Range(-shakeIntensity, shakeIntensity), 0, 0);
        }

        private static void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private static void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
