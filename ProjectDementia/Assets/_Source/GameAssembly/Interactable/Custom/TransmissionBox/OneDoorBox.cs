using System;
using DG.Tweening;
using Interactable.Custom.ClickInteractions;
using Photon.Pun;
using UnityEngine;

namespace Interactable.Custom.TransmissionBox
{
    public class OneDoorBox : MonoBehaviourPun
    {
        public PickableItem CurrentItem { get; private set; }

        [SerializeField] private ItemObjectPlaceZone itemObjectPlaceZone;
        [SerializeField] private float openDegrees;
        [SerializeField] private float openCloseTime;
        [SerializeField] private bool openOnStart = true;

        [Header("Player Door")] [SerializeField]
        private Transform firstDoorR;

        [SerializeField] private Transform firstDoorL;

        private bool _opened;
        private Sequence _currentAnim;

        public event Action OnItemPlacedEvent;

        private void Start()
        {
            Bind();

            if (!openOnStart)
                return;

            ToggleDoor(true);
        }

        private void OnDestroy() => Expose();

        public void ToggleDoor_RPC(bool toggle) => photonView.RPC(nameof(ToggleDoor), RpcTarget.All, toggle);

        [PunRPC]
        public void ToggleDoor(bool open)
        {
            if(_opened == open)
                return;
            
            _currentAnim.Kill();
            _currentAnim = DOTween.Sequence();
            _currentAnim.Append(
                firstDoorR.DOLocalRotate(open ? new Vector3(0, openDegrees, 0) : Vector3.zero,
                    openCloseTime));
            _currentAnim.Insert(0,
                firstDoorL.DOLocalRotate(open ? new Vector3(0, -openDegrees, 0) : Vector3.zero,
                    openCloseTime));
            _opened = open;
            CurrentItem?.SetInteractable(false);
        }

        private void OnItemPlaced(PickableItem item) // Called on both clients (network method)
        {
            CurrentItem = item;
            CurrentItem.SetInteractable(false);

            ToggleDoor(false);

            OnItemPlacedEvent?.Invoke();
        }

        private void Bind() => itemObjectPlaceZone.OnObjectPlaced += OnItemPlaced;

        private void Expose() => itemObjectPlaceZone.OnObjectPlaced -= OnItemPlaced;
    }
}