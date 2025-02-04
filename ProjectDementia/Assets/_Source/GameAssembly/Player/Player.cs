using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] public float walkSpeed = 2f;
        [SerializeField] public float runSpeed = 5f;
        [SerializeField] public float jumpHeight = 2f;

        private float _moveSpeed;
        private bool _isGrounded;

        public float Speed => walkSpeed;
        public float SprintSpeed => runSpeed;
    }
}
