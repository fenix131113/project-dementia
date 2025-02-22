using System;
using System.Collections.Generic;
using System.Linq;
using Interactable.Custom;
using Photon.Pun;
using Player;
using UnityEngine;

namespace Levels._1
{
    public class FloorPlatesGameManager : MonoBehaviour
    {
        [SerializeField] private PhotonView netView;
        [SerializeField] private Transform teleportPoint;
        [SerializeField] private Door[] doors;
        [SerializeField] private List<ObjectPressablePlate> teleportPlates;
        [SerializeField] private List<CorrectPlateItem> correctPlates;
        [SerializeField] private List<ObjectPressablePlate> defaultPlates;

        private int _counter;

        private void Start() => Bind();

        private void OnDestroy() => Expose();
        
        private void RPC_ResetGame() => netView.RPC(nameof(ResetGame), RpcTarget.All);
        
        [PunRPC]
        private void ResetGame()
        {
            _counter = 0;
            teleportPlates.ForEach(x => x.ResetPlate());
            correctPlates.ForEach(x =>
            {
                x.InfoScreen.ResetScreen();
                x.PressablePlate.ResetPlate();
            });
            defaultPlates.ForEach(x => x.ResetPlate());
        }

        [PunRPC]
        private void CheckGameWinConditions()
        {
            if (_counter == 4)
            {
                teleportPlates.ForEach(x => x.BlockPlate());
                correctPlates.ForEach(x => x.PressablePlate.BlockPlate());
                defaultPlates.ForEach(x => x.BlockPlate());

                foreach (var door in doors)
                    door.OpenDoor();
            }
        }

        private void ResetWithPlayer(ObjectPressablePlate pressablePlate, GameObject player)
        {
            RPC_ResetGame();
            player.GetComponent<PlayerController>().Teleport(teleportPoint.position);
        }

        private void OnCorrectPressablePlatePressed(ObjectPressablePlate pressablePlate)
        {
            var plateIndex = correctPlates.IndexOf(correctPlates.First(x => x.PressablePlate == pressablePlate));
            netView.RPC(nameof(SetScreenData), RpcTarget.All, plateIndex,
                (int)correctPlates[plateIndex].ActiveColor.r, (int)correctPlates[plateIndex].ActiveColor.g,
                (int)correctPlates[plateIndex].ActiveColor.b);

            netView.RPC(nameof(CheckGameWinConditions), RpcTarget.All);
        }

        [PunRPC]
        private void SetScreenData(int screenIndex, int r, int g, int b)
        {
            _counter++;
            correctPlates[screenIndex].InfoScreen.SetColor(r, g, b);
            correctPlates[screenIndex].InfoScreen.DrawText($"{_counter}/4");
        }

        private void Bind()
        {
            teleportPlates.ForEach(x => x.OnPressedOwner += ResetWithPlayer);
            correctPlates.ForEach(x => x.PressablePlate.OnPressed += OnCorrectPressablePlatePressed);
        }

        private void Expose()
        {
            teleportPlates.ForEach(x => x.OnPressedOwner -= ResetWithPlayer);
            correctPlates.ForEach(x => x.PressablePlate.OnPressed -= OnCorrectPressablePlatePressed);
        }
    }

    [Serializable]
    public class CorrectPlateItem
    {
        [field: SerializeField] public ObjectPressablePlate PressablePlate { get; set; }
        [field: SerializeField] public InfoScreen InfoScreen { get; set; }
        [field: SerializeField] public Color ActiveColor { get; set; }
    }
}