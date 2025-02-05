using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNickLabel;
        [SerializeField] private GameObject[] invisibleObjectsForMyPlayer;
        [SerializeField] private GameObject[] invisibleObjectsForOtherPlayer;
        public float Speed = 5f;
        public float SprintSpeed = 8f;
        public float JumpHeight = 2f;

        [PunRPC]
        private void DeactivateInvisibleForOther()
        {
            foreach (var obj in invisibleObjectsForOtherPlayer)
                obj.SetActive(false);
        }

        [PunRPC]
        public void SetNickname(string nickname)
        {
            gameObject.name = nickname;
            playerNickLabel.text = nickname;
        }
    }

}
