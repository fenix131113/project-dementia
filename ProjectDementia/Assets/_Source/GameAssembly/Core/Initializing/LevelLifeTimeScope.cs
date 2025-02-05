using InventorySystem;
using ItemsSystem;
using ItemsSystem.Data;
using UnityEngine;
using Utils.ResourcesLoading;
using VContainer;
using VContainer.Unity;

namespace Core.Initializing
{
    public class LevelLifeTimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LevelBootstrapper>();

            #region ResouresesLoading

            var dataRepository = new DataRepository<ScriptableObject>();
            dataRepository.Create(typeof(ItemSO), ResourceLoader.Load<ItemSO>(DataPath.ITEMS_CONFIGS_PATH));

            builder.RegisterInstance(dataRepository);

            #endregion

            #region InventorySystem

            builder.Register<ItemSelector>(Lifetime.Singleton).AsSelf().As<IStartable>();
            builder.Register<ItemsContainer>(Lifetime.Singleton);
            builder.Register<PlayersInventory>(Lifetime.Singleton);
            builder.Register<InventorySwitcher>(Lifetime.Singleton).AsSelf().As<ITickable>();

            #endregion
        }
    }
}