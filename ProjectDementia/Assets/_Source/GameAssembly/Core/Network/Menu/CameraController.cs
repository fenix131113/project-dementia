using UnityEngine;

namespace Settings
{
    public class CameraController : MonoBehaviour
    {
        public float sensitivity = 2.0f;
        public Transform playerBody;

        private float xRotation = 0f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            playerBody.Rotate(Vector3.up * mouseX);
        }

        public void SetSensitivity(float newSensitivity)
        {
            sensitivity = newSensitivity;
        }
    }
}