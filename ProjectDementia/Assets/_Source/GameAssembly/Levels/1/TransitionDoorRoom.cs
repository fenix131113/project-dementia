using Interactable.Custom;
using Photon.Pun;
using UnityEngine;

namespace Levels._1
{
    public class TransitionDoorRoom : MonoBehaviourPun
    {
        [SerializeField] private Door[] doorsToOpen;
        [SerializeField] private Door[] doorsToClose;
        [SerializeField] private ObjectPressablePlate[] plates;

        private bool _firstOnPlate;
        private bool _secondOnPlate;
        private bool _isTransitionCompleted;

        public void SetFirstPlayerOnPlate(bool state) =>
            photonView.RPC(nameof(SetFirstPlayerOnPlate_RPC), RpcTarget.All, state);

        public void SetSecondPlayerOnPlate(bool state) =>
            photonView.RPC(nameof(SetSecondPlayerOnPlate_RPC), RpcTarget.All, state);

        [PunRPC]
        public void SetFirstPlayerOnPlate_RPC(bool state)
        {
            _firstOnPlate = state;
            CheckConditions();
        }

        [PunRPC]
        public void SetSecondPlayerOnPlate_RPC(bool state)
        {
            _secondOnPlate = state;
            CheckConditions();
        }

        private void CheckConditions()
        {
            if (_isTransitionCompleted || !_firstOnPlate || !_secondOnPlate)
                return;

            _isTransitionCompleted = true;
            MakeTransition();
        }

        private void MakeTransition()
        {
            foreach (var door in doorsToClose)
                door.CloseDoor();

            foreach (var door in doorsToOpen)
                door.OpenDoor();

            foreach (var plate in plates)
                plate.BlockPlate();
        }
    }
}