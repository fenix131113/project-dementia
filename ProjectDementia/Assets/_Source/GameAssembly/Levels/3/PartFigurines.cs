using System;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;

namespace Levels._3
{
    public class PartFigurines : MonoBehaviourPun
    {
        public int CurrentRotateIndex { get; private set; }

        [SerializeField] private Transform partPivot;
        [SerializeField] private List<float> partRotations;
        [SerializeField] private float rotationTime;

        /// <summary>
        /// Invokes on both clients (network event)
        /// </summary>
        public event Action OnRotateChanged;

        private void Start()
        {
            partPivot.DOLocalRotate(new Vector3(-partRotations[CurrentRotateIndex], 0, 0), rotationTime);
        }

        private void OnDestroy() => OnRotateChanged = null;

        public void SwitchPartPosition_RPC() => photonView.RPC(nameof(SwitchPartPosition), RpcTarget.All);

        [PunRPC]
        private void SwitchPartPosition()
        {
            CurrentRotateIndex++;

            if (CurrentRotateIndex >= partRotations.Count)
                CurrentRotateIndex = 0;
            
            partPivot.DOLocalRotate(new Vector3(-partRotations[CurrentRotateIndex], 0, 0), rotationTime);
            OnRotateChanged?.Invoke();
        }
    }
}