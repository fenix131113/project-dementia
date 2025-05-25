using System;
using System.Collections.Generic;
using System.Linq;
using Interactable.Custom;
using Interactable.Custom.Screens;
using Photon.Pun;
using UnityEngine;

namespace Levels._1
{
    public class PlatePasswordManager : MonoBehaviourPun
    {
        [SerializeField] private List<PlateItems> colorPlates;
        [SerializeField] private Door[] doors;

        private int _pressedCounter;
        private readonly List<ObjectPressablePlate> _pressedPlates = new();

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void RPC_CheckPassword() => photonView.RPC(nameof(CheckPassword), RpcTarget.All);
        private void RPC_ResetPassword() => photonView.RPC(nameof(ResetPassword), RpcTarget.All);

        private void RPC_AddPressedPlate(int plateIndex) =>
            photonView.RPC(nameof(AddPressedPlate), RpcTarget.All, plateIndex);

        [PunRPC]
        private void CheckPassword()
        {
            if (_pressedPlates.Count != colorPlates.Count)
                return;

            if (colorPlates.Where((plate, index) => plate.PressablePlate != _pressedPlates[index]).Any())
                if (_pressedPlates.Count == colorPlates.Count)
                {
                    ResetPassword();
                    return;
                }

            //If password successful
            foreach (var plate in colorPlates)
                plate.PressablePlate.BlockPlate();

            foreach (var o in doors)
                o.OpenDoor();
        }

        [PunRPC]
        public void ResetPassword()
        {
            _pressedCounter = 0;
            _pressedPlates.Clear();

            foreach (var plate in colorPlates)
            {
                plate.PressablePlate.ResetPlate();
                plate.TextScreen.ResetScreen();
            }
        }

        [PunRPC]
        private void AddPressedPlate(int plateIndex)
        {
            _pressedCounter++;
            colorPlates[plateIndex].TextScreen.DrawText(_pressedCounter.ToString());
            _pressedPlates.Add(colorPlates[plateIndex].PressablePlate);
        }

        private void OnPlatePressed(ObjectPressablePlate passwordPressablePlate)
        {
            RPC_AddPressedPlate(colorPlates.FindIndex(x => x.PressablePlate == passwordPressablePlate));
            RPC_CheckPassword();
        }

        private void Bind()
        {
            foreach (var plate in colorPlates)
                plate.PressablePlate.OnPressed += OnPlatePressed;
        }

        private void Expose()
        {
            foreach (var plate in colorPlates)
                plate.PressablePlate.OnPressed -= OnPlatePressed;
        }

        [Serializable]
        public class PlateItems
        {
            [field: SerializeField] public ObjectPressablePlate PressablePlate { get; set; }
            [field: SerializeField] public TextScreen TextScreen { get; set; }
        }
    }
}