using Player;
using UnityEngine;

namespace Test
{
    public class DeathZone : MonoBehaviour
    {
        [SerializeField] private Transform teleportPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var player))
                player.Teleport(teleportPoint.position);
        }
    }
}