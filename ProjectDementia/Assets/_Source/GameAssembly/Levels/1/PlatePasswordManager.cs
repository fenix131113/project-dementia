using System.Collections.Generic;
using System.Linq;
using Interactable.Custom;
using Photon.Pun;
using UnityEngine;

namespace Levels._1
{
    [RequireComponent(typeof(PhotonNetwork))]
    public class PlatePasswordManager : MonoBehaviour
    {
        [SerializeField] private List<PasswordColorPlate> colorPlates;
        [SerializeField] private PhotonView netView;
        [SerializeField] private Door[] doors;

        private int _pressedCounter;
        private readonly List<PasswordColorPlate> _pressedPlates = new();

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void RPC_CheckPassword() => netView.RPC(nameof(CheckPassword), RpcTarget.AllBuffered);
        private void RPC_ResetPassword() => netView.RPC(nameof(ResetPassword), RpcTarget.AllBuffered);

        private void RPC_AddPressedPlate(int plateIndex) =>
            netView.RPC(nameof(AddPressedPlate), RpcTarget.AllBuffered, plateIndex);

        [PunRPC]
        private void CheckPassword()
        {
            if (_pressedPlates.Count != colorPlates.Count)
                return;

            if (colorPlates.Where((plate, index) => plate != _pressedPlates[index]).Any())
                if (_pressedPlates.Count == colorPlates.Count)
                {
                    ResetPassword();
                    return;
                }

            //If password successful
            foreach (var plate in colorPlates)
                plate.BlockPlate();

            foreach (var o in doors)
                o.OpenDoor();
        }

        [PunRPC]
        public void ResetPassword()
        {
            _pressedCounter = 0;
            _pressedPlates.Clear();

            foreach (var plate in colorPlates)
                plate.ResetPlate();
        }

        [PunRPC]
        private void AddPressedPlate(int plateIndex)
        {
            _pressedCounter++;
            colorPlates[plateIndex].InfoScreen.DrawText(_pressedCounter.ToString());
            _pressedPlates.Add(colorPlates[plateIndex]);
        }

        private void OnPlatePressed(PasswordColorPlate passwordPlate)
        {
            RPC_AddPressedPlate(colorPlates.IndexOf(passwordPlate));
            RPC_CheckPassword();
        }

        private void Bind()
        {
            foreach (var plate in colorPlates)
                plate.OnPressed += OnPlatePressed;
        }

        private void Expose()
        {
            foreach (var plate in colorPlates)
                plate.OnPressed -= OnPlatePressed;
        }
    }
}