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
            //TODO: Change ScriptableObject to item SO class
            dataRepository.Create(typeof(ScriptableObject), ResourceLoader.Load<ScriptableObject>(DataPath.ITEMS_CONFIGS_PATH));
            //TODO: Generate items by loaded SO data in items container

            #endregion
        }
    }
}