using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNickLabel;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float gravity;
        [SerializeField] private float sensitivity;
        [SerializeField] private PhotonView netView;
        [SerializeField] private bool canMove;
        [SerializeField] private bool canRotCam;
        [SerializeField] private GameObject[] invisibleObjectsForMyPlayer;
        [SerializeField] private GameObject[] invisibleObjectsForOtherPlayer;

        private Transform _playerCamera;
        private Vector3 _velocity;
        private float _currentSpeed;
        private float _xRot;
        private float _startCamY;
        private float _shakeSpeed;
        private float _shakeAmount;
        private float _previousHorCamRot;
        private CharacterController _characterController;

        /// <summary>
        /// Set player controll
        /// <list type="bullet"><c><b>False</b></c> - Off player controll and show cursor</list>
        /// <br><list type="bullet"><c><b>True</b></c> - On player controll and hide cursor</list></br>
        /// </summary>
        public bool Controll
        {
            set
            {
                if (value)
                {
                    HideCursor();
                    canRotCam = true;
                    canMove = true;
                }
                else
                {
                    ShowCursor();
                    canRotCam = false;
                    canMove = false;
                }
            }
        }


        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _playerCamera = transform.GetChild(0);
            _currentSpeed = walkSpeed;
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
            
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<CharacterController>());
        }

        [PunRPC]
        public void SetNickname(string nickname)
        {
            gameObject.name = nickname;
            playerNickLabel.text = nickname;
        }

        public void Teleport(Vector3 position, bool resetVelocity = false)
        {
            var temp = _characterController.enabled;
            _characterController.enabled = false;
            transform.position = position;
            
            if(resetVelocity)
                _velocity = Vector3.zero;
            
            _characterController.enabled = temp;
        }

        private void MovePlayerCamera()
        {
            if (!canRotCam)
                return;
            
            var mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            _xRot -= mouseInput.y * sensitivity;
            _xRot = Mathf.Clamp(_xRot, -70, 70);

            transform.Rotate(0, mouseInput.x * sensitivity, 0);
            _playerCamera.localRotation = Quaternion.Euler(_xRot, 0, _playerCamera.rotation.eulerAngles.z);
        }

        private void MovePlayer()
        {
            var moveVector =
                transform.TransformDirection(new Vector3(Input.GetAxisRaw("Horizontal"), 0,
                    Input.GetAxisRaw("Vertical")));

            //Shift run
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                _currentSpeed = runSpeed;
                _shakeAmount = 0.1f;
                _shakeSpeed = 13;
            }
            else
            {
                _currentSpeed = walkSpeed;
                _shakeAmount = 0.08f;
                _shakeSpeed = 8;
            }

            if (canMove)
            {
                #region Movement

                if (_characterController.isGrounded)
                {
                    _velocity.y = -1;

                    //if (Input.GetKeyDown(KeyCode.Space))
                    //{
                    //	velocity.y = jumpForce;
                    //}
                }
                else
                {
                    _velocity.y -= gravity * -2f * Time.deltaTime;
                }

                _characterController.Move(moveVector * (_currentSpeed * Time.deltaTime));
                _characterController.Move(_velocity * Time.deltaTime);

                #endregion
            }

            #region Player shake cam

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

            //Horizontal shake
            var targetTilt = 0f;
            if (Input.GetAxisRaw("Horizontal") > 0f)
            {
                targetTilt = -1;
            }
            else if (Input.GetAxisRaw("Horizontal") < 0f)
            {
                targetTilt = 1;
            }

            var tilt = Mathf.Lerp(_previousHorCamRot, targetTilt, 4f * Time.deltaTime);
            _previousHorCamRot = tilt;

            _playerCamera.rotation = Quaternion.Euler(_playerCamera.rotation.eulerAngles.x,
                _playerCamera.rotation.eulerAngles.y, tilt);

            #endregion
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