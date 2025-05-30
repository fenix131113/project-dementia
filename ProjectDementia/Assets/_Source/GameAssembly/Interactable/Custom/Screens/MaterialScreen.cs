using UnityEngine;

namespace Interactable.Custom.Screens
{
    public class MaterialScreen : MonoBehaviour
    {
        [field: SerializeField] public Material DefaultMaterial { get; private set; }

        [SerializeField] private MeshRenderer screenRenderer;

        private void Awake() => ResetScreen();

        public void ResetScreen() => screenRenderer.material = DefaultMaterial;

        public void SetMaterial(Material material) => screenRenderer.material = material;
    }
}