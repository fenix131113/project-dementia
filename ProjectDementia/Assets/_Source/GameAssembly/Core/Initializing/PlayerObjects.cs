using System.Collections.Generic;
using UnityEngine;

namespace Core.Initializing
{
    public class PlayerObjects
    {
        public GameObject PlayerObject { get; private set; }
        private readonly Dictionary<int, GameObject> _playerObjects = new();
        
        //public void RegisterPlayerObject(int actorNumber, GameObject playerObject)
        //{
        //    _playerObjects.Add(actorNumber, playerObject);
        //}

        //public GameObject GetPlayerObject(int actorNumber) => _playerObjects[actorNumber];

        public void SetCurrentPlayerObject(GameObject playerObject) => PlayerObject = playerObject;
    }
}