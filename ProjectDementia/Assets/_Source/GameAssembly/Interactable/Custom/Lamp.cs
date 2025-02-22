using UnityEngine;

namespace Interactable.Custom
{
    public class Lamp : MonoBehaviour
    {
        [SerializeField] private bool startState;
        [SerializeField] private Light lampLight;
        [SerializeField] private MeshRenderer lampRenderer;
        [SerializeField] private Material activeMaterial;
        [SerializeField] private Material inactiveMaterial;

        private void Start()
        {
            if (startState)
                Activate();
            else
                Deactivate();
        }

        public void Activate()
        {
            lampLight?.gameObject.SetActive(true);
            if (activeMaterial != null && lampRenderer != null)
                lampRenderer.material = activeMaterial;
        }

        public void Deactivate()
        {
            lampLight?.gameObject.SetActive(false);
            if (inactiveMaterial != null && lampRenderer != null)
                lampRenderer.material = inactiveMaterial;
        }
    }
}