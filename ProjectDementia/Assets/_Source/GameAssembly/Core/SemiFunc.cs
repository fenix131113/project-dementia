using Photon.Pun;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core
{
    public static class SemiFunc
    {
        private static IObjectResolver _resolver;

        public static void Init(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public static void InjectObject(GameObject obj)
        {
            _resolver.InjectGameObject(obj);
        }

        public static Photon.Realtime.Player GetPlayerByActorNumber(int actorNumber) =>
            PhotonNetwork.PlayerList[0].Get(actorNumber);
    }
}