using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Core.Network
{
    public class Level : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Transform firstSpawnPoint;
        [SerializeField] private Transform secondSpawnPoint;

        private IObjectResolver _resolver;

        private void Start()
        {
            _resolver = FindFirstObjectByType<LifetimeScope>().Container;
            SpawnPlayer();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            DOTween.KillAll();
            SceneManager.LoadScene(0);
        }

        private void SpawnPlayer()
        {
            var selectedSpawn = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? firstSpawnPoint : secondSpawnPoint;
            
            var spawned = PhotonNetwork.Instantiate("Prefabs/PlayerPrefab",
                selectedSpawn.position, Quaternion.identity);
            photonView.RPC(nameof(InjectSpawnedPlayer), RpcTarget.AllBuffered,
                spawned.GetComponent<PhotonView>().ViewID);
            
            spawned.GetComponent<PhotonView>()
                .RPC("SetNickname", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        }

        [PunRPC]
        private void InjectSpawnedPlayer(int viewID)
        {
            _resolver.InjectGameObject(PhotonNetwork.GetPhotonView(viewID).gameObject);
        }
    }
}