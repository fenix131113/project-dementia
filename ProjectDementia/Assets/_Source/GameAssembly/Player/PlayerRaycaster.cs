using Photon.Pun;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerRaycaster : MonoBehaviour
    {
        [SerializeField] private GameObject interactHelper;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private float rayDistance;

        public void Update()
        {
            if (!Physics.Raycast(transform.position, transform.forward, out var hit, rayDistance,
                    interactableLayer))
            {
                interactHelper.gameObject.SetActive(false);
                return;
            }
            
            interactHelper.gameObject.SetActive(true);

            if(!Input.GetKeyDown(KeyCode.E))
                return;
            
            if (hit.transform.gameObject &&
                LayerService.CheckLayersEquality(hit.transform.gameObject.layer, interactableLayer))
                hit.transform.GetComponent<PhotonView>().RPC("KillObject", RpcTarget.AllBuffered);
        }
    }
}