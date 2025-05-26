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

        private void Start()
        {
            Bind();

            if (!openOnStart)
                return;

            if (firstOpen)
                ToggleFirst_RPC(true);
            else
                ToggleSecond_RPC(true);
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

        public void ToggleFirst_RPC(bool open) => photonView.RPC(nameof(ToggleFirst), RpcTarget.All, open);

        public void ToggleSecond_RPC(bool open) => photonView.RPC(nameof(ToggleSecond), RpcTarget.All, open);

        [PunRPC]
        public void ToggleFirst(bool open)
        {
            firstDoorR.DORotate(open ? new Vector3(0, openDegrees, 0) : Vector3.zero, openCloseTime);
            firstDoorL.DORotate(open ? new Vector3(0, -openDegrees, 0) : Vector3.zero, openCloseTime);
            _firstOpened = open;
        }

        [PunRPC]
        public void ToggleSecond(bool open)
        {
            secondDoorR.DORotate(open ? new Vector3(0, -openDegrees, 0) : Vector3.zero, openCloseTime);
            secondDoorL.DORotate(open ? new Vector3(0, openDegrees, 0) : Vector3.zero, openCloseTime);
            _firstOpened = !open;
        }

        private void OnItemPlaced(PickableItem item)
        {
            CurrentItem = item;
            CurrentItem.SetInteractable(false);

            var temp = _firstOpened;

            if (temp)
                ToggleFirst_RPC(false);
            else
                ToggleSecond_RPC(false);

            if (openInstantly)
                StartCoroutine(DoorToggleCoroutine(temp));
        }

        private void Bind() => itemObjectPlaceZone.OnObjectPlaced += OnItemPlaced;

        private void Expose() => itemObjectPlaceZone.OnObjectPlaced -= OnItemPlaced;

        private IEnumerator DoorToggleCoroutine(bool firstOpened)
        {
            yield return new WaitForSeconds(openCloseTime);

            if (firstOpened)
                ToggleSecond_RPC(true);
            else
                ToggleFirst_RPC(true);

            yield return new WaitForSeconds(openCloseTime);

            CurrentItem.SetInteractable(true);
            ClearCurrentItem();
        }
    }
}