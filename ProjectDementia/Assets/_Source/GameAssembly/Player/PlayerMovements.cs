using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovements : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private TMP_Text playerNickLabel;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float sensitivity = 2f;
        [SerializeField] private PhotonView netView;
        [SerializeField] private GameObject[] invisibleObjectsForMyPlayer;
        [SerializeField] private GameObject[] invisibleObjectsForOtherPlayer;

        private Vector2 _moveDirection;
        private bool isDead;
        private Transform _playerCamera;
        private CharacterController _characterController;
        private Vector3 _velocity;
        private float _currentSpeed;
        private float _xRot;
        private float _startCamY;
        private float _shakeSpeed;
        private float _shakeAmount;
        private float _previousHorCamRot;

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

            foreach (var obj in invisibleObjectsForMyPlayer)
                obj.SetActive(false);

            netView.RPC(nameof(DeactivateInvisibleForOther), RpcTarget.OthersBuffered);

            HideCursor();
        }

        private void Update()
        {
            if (!netView.IsMine)
                return;

            MovePlayer();
            MovePlayerCamera();
        }

        [PunRPC]
        private void DeactivateInvisibleForOther()
        {
            foreach (var obj in invisibleObjectsForOtherPlayer)
                obj.SetActive(false);
        }

        [PunRPC]
        public void SetNickname(string nickname)
        {
            gameObject.name = nickname;
            playerNickLabel.text = nickname;
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveDirection = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && !isDead && _characterController.isGrounded)
            {
                Jump();
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SetSprintState(true);
            }
            else if (context.canceled)
            {
                SetSprintState(false);
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed && !isDead)
            {
                Debug.Log("Interaction");
            }
        }

        private void SetSprintState(bool isSprinting)
        {
            _currentSpeed = isSprinting ? _player.SprintSpeed : _player.Speed;
        }

        private void MovePlayerCamera()
        {
            if (!netView.IsMine) return;

            var mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            _xRot -= mouseInput.y * sensitivity;
            _xRot = Mathf.Clamp(_xRot, -70, 70);

            transform.Rotate(0, mouseInput.x * sensitivity, 0);
            _playerCamera.localRotation = Quaternion.Euler(_xRot, 0, _playerCamera.rotation.eulerAngles.z);
        }

        private void MovePlayer()
        {
            if (isDead) return;

            var moveVector = transform.TransformDirection(new Vector3(_moveDirection.x, 0, _moveDirection.y));
            _currentSpeed = _currentSpeed == 0 ? _player.Speed : _currentSpeed;

            if (_characterController.isGrounded)
            {
                _velocity.y = -1;
            }
            else
            {
                _velocity.y += gravity * Time.deltaTime;
            }

            _characterController.Move(moveVector * (_currentSpeed * Time.deltaTime));
            _characterController.Move(_velocity * Time.deltaTime);

            if ((moveVector.x != 0 || moveVector.z != 0) && _characterController.isGrounded)
            {
                _playerCamera.localPosition = Vector3.Lerp(_playerCamera.localPosition,
                    new Vector3(_playerCamera.localPosition.x,
                        _startCamY - Mathf.Sin(Time.time * _shakeSpeed) * _shakeAmount, _playerCamera.localPosition.z),
                    Time.deltaTime * 10f);
            }
            else
            {
                _playerCamera.localPosition = new Vector3(_playerCamera.localPosition.x,
                    Mathf.Lerp(_playerCamera.localPosition.y, _startCamY, Time.deltaTime * 10f),
                    _playerCamera.localPosition.z);
            }
        }

        private void Jump()
        {
            _velocity.y = Mathf.Sqrt(-2f * gravity * _player.jumpHeight);
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