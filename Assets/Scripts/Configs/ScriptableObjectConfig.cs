using Core;
using Core.ResourceManagement;
using UnityEngine;

namespace Configs
{
    public interface IScriptableObjectConfig<out TConfigType>
    where TConfigType : ScriptableObject
    {
        TConfigType Value { get; }
    }

    public abstract class ScriptableObjectConfig<TConfigType> : IScriptableObjectConfig<TConfigType>
        where TConfigType : ScriptableObject
    {
        #region - Properties
        public TConfigType Value => config ? config : config = LoadConfig();
        #endregion

        #region - State
        private TConfigType config;
        #endregion

        #region - Private
        protected abstract string ConfigPath { get; }

        private TConfigType LoadConfig()
        {
            var resourceManager = GameContext.Container.Resolve<IResourceManager>();
            return resourceManager.LoadResource<TConfigType>(ConfigPath);
        }
        #endregion
    }
}