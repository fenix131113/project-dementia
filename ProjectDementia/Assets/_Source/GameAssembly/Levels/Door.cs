using DG.Tweening;
using UnityEngine;

namespace Levels
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private float doorOpenTime = 2f;
        [SerializeField] private float doorOpenWidth = 2f;
        [SerializeField] private Transform doorPart;
        [SerializeField] private Transform secondDoorPart;
        
        private float _startFirstDoorZPosition;
        private float _startSecondDoorZPosition;

        private void Start()
        {
            _startFirstDoorZPosition = doorPart.localPosition.z;
            _startSecondDoorZPosition = secondDoorPart.localPosition.z;
        }

        public void OpenDoor()
        {
            doorPart.DOLocalMoveZ(doorPart.localPosition.z - doorOpenWidth, doorOpenTime);
            secondDoorPart.DOLocalMoveZ(secondDoorPart.localPosition.z + doorOpenWidth, doorOpenTime);
        }

        public void CloseDoor()
        {
            doorPart.DOLocalMoveZ(_startFirstDoorZPosition, doorOpenTime);
            secondDoorPart.DOLocalMoveZ(_startSecondDoorZPosition, doorOpenTime);
        }
    }
}