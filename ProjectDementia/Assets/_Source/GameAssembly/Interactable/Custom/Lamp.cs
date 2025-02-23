using Photon.Pun;
using UnityEngine;

namespace Interactable.Custom
{
    [RequireComponent(typeof(PhotonView))]
    public class Lamp : MonoBehaviour
    {
        [SerializeField] private PhotonView netView;
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
        
        public void RPC_Activate(RpcTarget target) => netView.RPC(nameof(Activate), target);
        public void RPC_Deactivate(RpcTarget target) => netView.RPC(nameof(Deactivate), target);

        [ContextMenu("Activate")]
        public void Activate()
        {
            lampLight?.gameObject.SetActive(true);
            if (activeMaterial && lampRenderer)
                lampRenderer.material = activeMaterial;
        }

        [ContextMenu("Deactivate")]
        public void Deactivate()
        {
            lampLight?.gameObject.SetActive(false);
            if (inactiveMaterial && lampRenderer)
                lampRenderer.material = inactiveMaterial;
        }
    }
}