using Photon.Pun;
using UnityEngine;

namespace Player
{
    public class CameraControll : MonoBehaviour
    {
        [SerializeField] private float cameraSmoothSpeed = 5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float sensitivity = 2f;

        [SerializeField] private PhotonView netView;

        [SerializeField] private bool canMove;
        [SerializeField] private bool canRotCam;

        private CharacterController _characterController;
        private Transform _playerCamera;
        private Vector2 _lookInput;
        private float _xRot;
        private float _startCamY;


        public bool Controll
        {
            set
            {
                if (value)
                {
                    canRotCam = true;
                    canMove = true;
                }
                else
                {
                    canRotCam = false;
                    canMove = false;
                }
            }
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerCamera = transform.GetChild(0);
            _startCamY = _playerCamera.localPosition.y;

            if (!netView.IsMine)
            {
                _playerCamera.gameObject.SetActive(false);
                return;
            }
        }

        private void Update()
        {
            if (!netView.IsMine)
                return;

            MovePlayerCamera();
        }

        private void MovePlayerCamera()
        {
            if (!netView.IsMine || !canRotCam)
                return;

            _xRot -= _lookInput.y * sensitivity;
            _xRot = Mathf.Clamp(_xRot, -70, 70);

            transform.Rotate(0, _lookInput.x * sensitivity, 0);

            Quaternion targetCameraRotation = Quaternion.Euler(_xRot, 0, 0);
            _playerCamera.localRotation = Quaternion.Slerp(_playerCamera.localRotation, targetCameraRotation, Time.deltaTime * cameraSmoothSpeed);
        }
    }

}