using Photon.Pun;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Network
{
    public class Level : MonoBehaviourPunCallbacks
    {
        private IObjectResolver _resolver;
        
        private void Start()
        {
            _resolver = FindFirstObjectByType<LifetimeScope>().Container;
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            var spawned = PhotonNetwork.Instantiate("Prefabs/PlayerPrefab",
                new Vector3(Random.Range(-3f, 3f), Random.Range(1f, 3f), Random.Range(-3f, 3f)), Quaternion.identity);
            photonView.RPC(nameof(InjectSpawnedPlayer), RpcTarget.AllBuffered, spawned.GetComponent<PhotonView>().ViewID);
            spawned.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        }

        [PunRPC]
        private void InjectSpawnedPlayer(int viewID)
        {
            _resolver.InjectGameObject(PhotonNetwork.GetPhotonView(viewID).gameObject);
        }
    }
}