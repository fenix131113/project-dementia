using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Player
{
    public class PlayerMovements : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _sprintMultiplier = 1.5f;
        [SerializeField] private float _jumpForce = 5f;
        [SerializeField] private bool isCrouching = false;

        private Vector2 _moveDirection;
        private bool isMoving = true;
        private bool isGrounded = true;

        private Rigidbody rb;

        [SerializeField] private GameObject settingsMenu;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _moveDirection = context.ReadValue<Vector2>();
            if (isMoving)
            {
                Move(_moveDirection);
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && isGrounded)
            {
                Jump();
            }
        }

        private void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isCrouching = !isCrouching;
            }
        }

        private void OnEscape(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ToggleSettingsMenu();
            }
        }

        private void Update()
        {
            if (isMoving)
            {
                Move(_moveDirection);
            }
        }

        private void Move(Vector2 direction)
        {
            float scaledMoveSpeed = _moveSpeed * (isCrouching ? 0.5f : 1f) * Time.deltaTime;
            if (Keyboard.current.leftShiftKey.isPressed)
            {
                scaledMoveSpeed *= _sprintMultiplier;
            }

            Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);
            rb.MovePosition(transform.position + moveDirection * scaledMoveSpeed);
        }

        private void Jump()
        {
            rb.AddForce(new Vector3(0, _jumpForce, 0), ForceMode.Impulse);
            isGrounded = false;
        }

        private void ToggleSettingsMenu()
        {
            if (settingsMenu != null)
            {
                bool isActive = settingsMenu.activeSelf;
                settingsMenu.SetActive(!isActive);
                isMoving = isActive;
            }
        }
    }
}
