using Core;
using ItemsSystem.Data;
using Photon.Pun;
using UnityEngine;

namespace Levels._3
{
    public class Pipe : MonoBehaviourPun
    {
        [SerializeField] private Transform dropPoint;
        [SerializeField] private LayerMask objectExcludeLayers;

        public void DropItem(ItemSO item)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            var spawned = PhotonNetwork.Instantiate(
                $"{FoldersPaths.PICKABLE_PREFABS_PATH}{item.AdditionalFolderPath}/{item.Prefab.name}",
                dropPoint.position, Quaternion.identity).GetComponent<PhotonView>();
                
            photonView.RPC(nameof(InitSpawnedItem), RpcTarget.All, spawned.ViewID);
        }

        [PunRPC]
        private void InitSpawnedItem(int viewId)
        {
            var spawned = PhotonView.Find(viewId).gameObject;
            SemiFunc.InjectObject(spawned);

            spawned.AddComponent<Rigidbody>().excludeLayers = objectExcludeLayers;
        }
    }
}