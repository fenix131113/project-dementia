using Photon.Pun;
using UnityEngine;

namespace Test
{
    public class DestroyableObject : MonoBehaviour
    {
        [PunRPC]
        public void KillObject()
        {
            Destroy(gameObject);
        }
    }
}