using Photon.Pun;
using UnityEngine;
using Utils;

namespace Test
{
    public class LobbyReturnZone : MonoBehaviour
    {
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private int levelIndex;

        public void Load() => PhotonNetwork.LoadLevel(levelIndex);

        private void OnTriggerEnter(Collider other)
        {
            if(!LayerService.CheckLayersEquality(other.gameObject.layer, interactableLayer))
                return;
            
            Load();
        }
    }
}