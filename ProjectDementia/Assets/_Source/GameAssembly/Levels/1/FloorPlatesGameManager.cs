using System;
using System.Collections.Generic;
using System.Linq;
using Interactable.Custom;
using Interactable.Custom.Screens;
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

        private int _counter;

        private void Start() => Bind();

        private void OnDestroy() => Expose();
        
        private void ResetGame_RPC() => netView.RPC(nameof(ResetGame), RpcTarget.All);
        
        [PunRPC]
        private void ResetGame()
        {
            _counter = 0;
            teleportPlates.ForEach(x => x.ResetPlate());
            correctPlates.ForEach(x =>
            {
                x.TextScreen.ResetScreen();
                x.PressablePlate.ResetPlate();
            });
        }

        [PunRPC]
        private void CheckGameWinConditions()
        {
            if (_counter != 4)
                return;
            
            teleportPlates.ForEach(x => x.BlockPlate());
            correctPlates.ForEach(x => x.PressablePlate.BlockPlate());

            foreach (var door in doors)
                door.OpenDoor();
        }

        private void ResetWithPlayer(ObjectPressablePlate pressablePlate, GameObject player)
        {
            ResetGame_RPC();
            player.GetComponent<PlayerController>().Teleport(teleportPoint.position);
        }

        private void OnCorrectPressablePlatePressed(ObjectPressablePlate pressablePlate)
        {
            Debug.Log("Correct");
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
            correctPlates[screenIndex].TextScreen.SetColor(r, g, b);
            correctPlates[screenIndex].TextScreen.DrawText($"{_counter}/4");
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
        [field: SerializeField] public TextScreen TextScreen { get; set; }
        [field: SerializeField] public Color ActiveColor { get; set; }
    }
}