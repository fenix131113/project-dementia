using System;
using System.Collections;
using DG.Tweening;
using Interactable.Custom.ClickInteractions;
using Photon.Pun;
using UnityEngine;

namespace Interactable.Custom.TransmissionBox
{
    public class BlockableTransitionBox : MonoBehaviourPun
    {
        public PickableItem CurrentItem { get; private set; }

        [SerializeField] private ItemObjectPlaceZone itemObjectPlaceZone;
        [SerializeField] private float openDegrees;
        [SerializeField] private float openCloseTime;
        [SerializeField] private bool openOnStart = true;
        [SerializeField] private bool firstOpen;
        [SerializeField] private bool openInstantly = true;

        [Header("First Player Door")] [SerializeField]
        private Transform firstDoorR;

        [SerializeField] private Transform firstDoorL;

        [Header("Second Player Door")] [SerializeField]
        private Transform secondDoorR;

        [SerializeField] private Transform secondDoorL;

        private bool _firstOpened;
        private Sequence _currentAnim;

        public event Action OnItemPlacedEvent;

        private void Start()
        {
            Bind();

            if (!openOnStart)
                return;

            if (firstOpen)
                ToggleFirst_RPC(true, false);
            else
                ToggleSecond_RPC(true, false);
        }

        private void OnDestroy() => Expose();

        /// <summary>
        /// Destroy and set CurrentItem to null
        /// </summary>
        public void DestroyCurrentItem()
        {
            Destroy(CurrentItem.gameObject);
            CurrentItem = null;
        }

        /// <summary>
        /// Set CurrentItem to null
        /// </summary>
        public void ClearCurrentItem() => CurrentItem = null;

        public void ToggleBoxActivation(bool state) => itemObjectPlaceZone.gameObject.SetActive(state);

        public void ToggleFirst_RPC(bool open, bool openNext) =>
            photonView.RPC(nameof(ToggleFirst), RpcTarget.All, open, openNext);

        public void ToggleSecond_RPC(bool open, bool openNext) =>
            photonView.RPC(nameof(ToggleSecond), RpcTarget.All, open, openNext);

        [PunRPC]
        public void ToggleFirst(bool open, bool openNext)
        {
            _currentAnim.Kill();
            _currentAnim = DOTween.Sequence();
            _currentAnim.Append(
                firstDoorR.DORotate(open ? new Vector3(0, openDegrees, 0) : Vector3.zero, openCloseTime));
            _currentAnim.Insert(0,
                firstDoorL.DORotate(open ? new Vector3(0, -openDegrees, 0) : Vector3.zero, openCloseTime));
            _firstOpened = open;

            if (!openNext && open)
                StartCoroutine(AllowTakeItemCoroutine());

            if (!openNext)
                return;

            CurrentItem?.SetInteractable(false);
            StartCoroutine(DoorToggleCoroutine(!open));
        }

        [PunRPC]
        public void ToggleSecond(bool open, bool openNext)
        {
            _currentAnim.Kill();
            _currentAnim = DOTween.Sequence();
            _currentAnim.Append(secondDoorR.DORotate(open ? new Vector3(0, -openDegrees, 0) : Vector3.zero,
                openCloseTime));
            _currentAnim.Insert(0,
                secondDoorL.DORotate(open ? new Vector3(0, openDegrees, 0) : Vector3.zero, openCloseTime));
            _firstOpened = !open;

            if (!openNext && open)
                StartCoroutine(AllowTakeItemCoroutine());

            if (!openNext)
                return;

            CurrentItem?.SetInteractable(false);
            StartCoroutine(DoorToggleCoroutine(open));
        }

        private void OnItemPlaced(PickableItem item) // Called on both clients (network method)
        {
            CurrentItem = item;
            CurrentItem.SetInteractable(false);

            var temp = _firstOpened;

            if (temp)
                ToggleFirst(false, openInstantly);
            else
                ToggleSecond(false, openInstantly);

            OnItemPlacedEvent?.Invoke();
        }

        private void Bind() => itemObjectPlaceZone.OnObjectPlaced += OnItemPlaced;

        private void Expose() => itemObjectPlaceZone.OnObjectPlaced -= OnItemPlaced;

        private IEnumerator DoorToggleCoroutine(bool firstOpened)
        {
            yield return new WaitForSeconds(openCloseTime);
            
            if (firstOpened)
                ToggleSecond(true, false);
            else
                ToggleFirst(true, false);

            StartCoroutine(AllowTakeItemCoroutine());
        }

        private IEnumerator AllowTakeItemCoroutine()
        {
            yield return new WaitForSeconds(openCloseTime);

            CurrentItem?.SetInteractable(true);
        }
    }
}