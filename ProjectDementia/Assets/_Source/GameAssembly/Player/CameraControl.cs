using Photon.Pun;
using UnityEngine;

namespace Player
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private PhotonView punView;
        [SerializeField] private float sensitivity;
        [SerializeField] private Transform playerBody;

        private float _rotationX;
        private float _rotationY;
        
        private void Start()
        {
            if (!punView.IsMine)
            {
                gameObject.SetActive(false);
                return;
            }
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }


        private void Update()
        {
            var mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

            transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}