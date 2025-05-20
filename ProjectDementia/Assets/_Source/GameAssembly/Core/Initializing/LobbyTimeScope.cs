using Core.Network.Lobby;
using VContainer;
using VContainer.Unity;

namespace Core.Initializing
{
    public class LobbyTimeScope : LevelLifeTimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            builder.RegisterComponentInHierarchy<ReadyManager>();
        }
    }
}