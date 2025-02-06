using DG.Tweening;
using UnityEngine;

namespace Levels
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private float doorOpenTime = 2f;
        [SerializeField] private float doorOpenHeight = 2f;

        public void OpenDoor() => transform.DOMoveY(transform.position.y + doorOpenHeight, doorOpenTime);
    }
}