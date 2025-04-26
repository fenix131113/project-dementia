using Photon.Pun;

namespace Core
{
    public static class SemiFunc
    {
        public static Photon.Realtime.Player GetPlayerByActorNumber(int actorNumber) =>
            PhotonNetwork.PlayerList[0].Get(actorNumber);
    }
}