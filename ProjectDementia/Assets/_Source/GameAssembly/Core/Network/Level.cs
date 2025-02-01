using Photon.Pun;
using UnityEngine;

namespace Core.Network
{
    public class Level : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            var spawned = PhotonNetwork.Instantiate("Prefabs/PlayerPrefab",
                new Vector3(Random.Range(-3f, 3f), Random.Range(1f, 3f), Random.Range(-3f, 3f)), Quaternion.identity);

            spawned.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        }
    }
}