using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovements : MonoBehaviour
    {
        private const float GRAVITY = -9.81f;
    
        [SerializeField] private PhotonView netView;
        [SerializeField] private PlayerStats playerStats;
        
        private CharacterController _characterController;
        private Vector3 _velocity;
        private Vector3 _moveDirection;
        private float _currentSpeed;
        private bool _isDead;
        private bool _canMove = true;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Debug.Log("false");
            if (!netView.IsMine || !_canMove)
                return;
            Debug.Log("True");
            
            MovePlayer();
        }

        public void SetMovementStatus(bool canMove) => _canMove = canMove;

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveDirection = context.ReadValue<Vector3>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && !_isDead && _characterController.isGrounded)
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

        private void SetSprintState(bool isSprinting)
        {
            _currentSpeed = isSprinting ? playerStats.SprintSpeed : playerStats.Speed;
        }

        private void MovePlayer()
        {
            if (_isDead)
                return;
            
            Debug.Log(_moveDirection.ToString());

            var moveVector = transform.TransformDirection(new Vector3(_moveDirection.x, 0, _moveDirection.z));
            _currentSpeed = _currentSpeed == 0 ? playerStats.Speed : _currentSpeed;

            if (_characterController.isGrounded)
            {
                _velocity.y = -1;
            }
            else
            {
                _velocity.y += GRAVITY * Time.deltaTime;
            }

            _characterController.Move(moveVector * (_currentSpeed * Time.deltaTime));
            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void Jump()
        {
            _velocity.y = Mathf.Sqrt(-2f * GRAVITY * playerStats.JumpHeight);
        }
    }
}
