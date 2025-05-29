using UnityEngine;

namespace Settings
{
    public class CameraController : MonoBehaviour
    {
        [Header("Mouse Settings")]
        public float sensitivity = 2.0f;
        public float minSensitivity = 0.1f;
        public float maxSensitivity = 10f;

        [Header("References")]
        public Transform playerBody;

        public bool CanRotate { get; set; } = true;

        private float xRotation = 0f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (!CanRotate) return;

            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }

        public void UpdateSensitivity(float newSensitivity)
        {
            sensitivity = Mathf.Clamp(newSensitivity, minSensitivity, maxSensitivity);
        }
    }
}