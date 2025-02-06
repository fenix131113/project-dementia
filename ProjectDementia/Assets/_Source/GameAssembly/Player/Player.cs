using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] public float walkSpeed = 2f;
        [SerializeField] public float runSpeed = 5f;
        [SerializeField] public float jumpHeight = 2f;

        [SerializeField] private TMP_Text playerNickLabel;
        [SerializeField] private PhotonView netView;

        [SerializeField] private GameObject[] invisibleObjectsForMyPlayer;
        [SerializeField] private GameObject[] invisibleObjectsForOtherPlayer;

        private float _moveSpeed;
        private bool _isGrounded;

        public float Speed => walkSpeed;
        public float SprintSpeed => runSpeed;

        public bool CursorControll
        {
            set
            {
                if (value)
                {
                    HideCursor();
                }
                else
                {
                    ShowCursor();
                }
            }
        }

        private void Awake()
        {
            foreach (var obj in invisibleObjectsForMyPlayer)
                obj.SetActive(false);

            netView.RPC(nameof(DeactivateInvisibleForOther), RpcTarget.OthersBuffered);
            HideCursor();
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

        private static void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private static void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
