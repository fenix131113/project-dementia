using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {
        [field: SerializeField] public float Speed { get; private set; } = 5f;
        [field: SerializeField] public float SprintSpeed { get; private set; } = 8f;
        [field: SerializeField] public float JumpHeight { get; private set; } = 2f;
        
        [SerializeField] private PhotonView netView;
        [SerializeField] private TMP_Text playerNickLabel;
        [SerializeField] private GameObject[] invisibleObjectsForMyPlayer;
        [SerializeField] private GameObject[] invisibleObjectsForOtherPlayer;

        private void Awake()
        {
            foreach (var obj in invisibleObjectsForMyPlayer)
                obj.SetActive(false);

            netView.RPC(nameof(DeactivateInvisibleForOther), RpcTarget.OthersBuffered);
        }

        [PunRPC]
        private void DeactivateInvisibleForOther()
        {
            foreach (var obj in invisibleObjectsForOtherPlayer)
                obj.SetActive(false);

            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<CharacterController>());
        }

        [PunRPC]
        public void SetNickname(string nickname)
        {
            gameObject.name = nickname;
            playerNickLabel.text = nickname;
        }
    }
}