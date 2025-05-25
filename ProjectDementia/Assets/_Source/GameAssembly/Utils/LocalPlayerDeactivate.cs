using Photon.Pun;
using UnityEngine;

namespace Utils
{
    public class LocalPlayerDeactivate : MonoBehaviour
    {
        [SerializeField] private bool masterDeactivate;
        [SerializeField] private GameObject[] objectsToDeactivate;

        private void Awake()
        {
            switch (masterDeactivate)
            {
                case true when PhotonNetwork.IsMasterClient:
                case false when !PhotonNetwork.IsMasterClient:
                {
                    foreach (var obj in objectsToDeactivate)
                        obj.SetActive(false);
                    break;
                }
            }
        }
    }
}