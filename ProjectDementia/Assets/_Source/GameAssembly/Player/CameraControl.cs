using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private float sensitivity = 2f;
        [SerializeField] private float smoothTime = 0.1f;
        [SerializeField] private float shakeIntensity = 0.1f;
        [SerializeField] private PhotonView netView;

        private CharacterController _characterController;
        private Vector2 _lookInput;
        private Vector2 _currentLook;
        private Vector2 _lookVelocity;
        private bool _canRotateCamera = true;
        private float _xRot;
        private float _startCamY;
        private float _shakeSpeed;
        private float _shakeAmount;
        private float _previousHorCamRot;
        private Vector3 _movementVector;

        private void Awake()
        {
            _startCamY = transform.localPosition.y;
            _characterController = GetComponent<CharacterController>();
            HideCursor();
        }

        private void Update()
        {
            if (!netView.IsMine || !_canRotateCamera)
                return;

            SmoothCameraRotation();
            CameraMovementShake();
        }

        public void SetMovementStatus(bool canRotateCamera) => _canRotateCamera = canRotateCamera;

        public void OnMouseDelta(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>() * sensitivity;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _movementVector = context.ReadValue<Vector3>();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _shakeAmount = 0.1f;
                _shakeSpeed = 13;
            }
            else if (context.canceled)
            {
                _shakeAmount = 0.08f;
                _shakeSpeed = 8;
            }
        }

        private void CameraMovementShake()
        {
            #region Player shake cam

            if ((_movementVector.x != 0 || _movementVector.z != 0) && _characterController.isGrounded)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition,
                    new Vector3(transform.localPosition.x,
                        _startCamY - Mathf.Sin(Time.time * _shakeSpeed) * _shakeAmount, transform.localPosition.z),
                    Time.deltaTime * 10f);
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x,
                    Mathf.Lerp(transform.localPosition.y, _startCamY, Time.deltaTime * 10f),
                    transform.localPosition.z);
            }

            //Horizontal shake
            var targetTilt = _movementVector.x switch
            {
                > 0f => -1,
                < 0f => 1,
                _ => 0f
            };

            var tilt = Mathf.Lerp(_previousHorCamRot, targetTilt, 4f * Time.deltaTime);
            _previousHorCamRot = tilt;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y, tilt);

            #endregion
        }

        private void SmoothCameraRotation()
        {
            _currentLook = Vector2.SmoothDamp(_currentLook, _lookInput, ref _lookVelocity, smoothTime);

            _xRot -= _currentLook.y;
            _xRot = Mathf.Clamp(_xRot, -70, 70);

            transform.Rotate(0, _currentLook.x, 0);
            transform.localRotation = Quaternion.Euler(_xRot + Random.Range(-shakeIntensity, shakeIntensity), 0, 0);
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