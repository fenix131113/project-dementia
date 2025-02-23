using DG.Tweening;
using UnityEngine;

namespace Levels
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private float doorOpenTime = 2f;
        [SerializeField] private float doorOpenHeight = 2f;
        
        private float _startDoorYPosition;

        private void Start() => _startDoorYPosition = transform.position.y;

        public void OpenDoor() => transform.DOMoveY(transform.position.y + doorOpenHeight, doorOpenTime);
        public void CloseDoor() => transform.DOMoveY(_startDoorYPosition, doorOpenTime);
    }
}