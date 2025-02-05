using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Player;

public class PlayerMovements : MonoBehaviour
{
    private Vector2 _moveDirection;
    private bool isDead;
    private CharacterController _characterController;
    private Vector3 _velocity;
    private float gravity = -9.81f;
    private float _currentSpeed;
    [SerializeField] private PhotonView netView;
    [SerializeField] private PlayerStats _playerStats;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!netView.IsMine) return;
        MovePlayer();
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

    private void SetSprintState(bool isSprinting)
    {
        _currentSpeed = isSprinting ? _playerStats.SprintSpeed : _playerStats.Speed;
    }

    private void MovePlayer()
    {
        if (isDead) return;

        var moveVector = transform.TransformDirection(new Vector3(_moveDirection.x, 0, _moveDirection.y));
        _currentSpeed = _currentSpeed == 0 ? _playerStats.Speed : _currentSpeed;

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
    }

    private void Jump()
    {
        _velocity.y = Mathf.Sqrt(-2f * gravity * _playerStats.JumpHeight);
    }
}
