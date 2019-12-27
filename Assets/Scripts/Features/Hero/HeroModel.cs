using Core.IoC;
using Core.ResourceManagement;
using UnityEngine;

namespace Features.Heroes
{
    public interface IHeroModel
    {
        Vector3 HeroPosition { get; }
        int GetCurrentHeroAttack { get; }

        Hero CreateHero(Transform parent);
    }

    public enum HeroState
    {
        Alive,
        Dead,
    }

    public class HeroModel : IHeroModel
    {
        #region Constants
        private const string DefaultHeroId = "archer_green";
        private const string DefaultHeroPath = "HeroPrefabs/archer_green";
        #endregion

        #region Dependencies
        [Inject] private IJsonConfig<HeroConfig> heroConfig;
        [Inject] private IResourceManager resourceManager;
        #endregion

        #region Properties
        private Hero heroInstance;
        public Vector3 HeroPosition => heroInstance.Position;
        #endregion

        #region State
        private int currentHealth;
        private HeroState currentState;
        #endregion
        #region Public
        public Hero CreateHero(Transform parent)
        {
            var heroTemplate = resourceManager.LoadResource<Hero>(DefaultHeroPath);
            var heroSettings = GetHeroBaseSettings();

            heroInstance = Object.Instantiate(heroTemplate, parent);
            heroInstance.Settings = heroSettings;
            return heroInstance;
        }
        public int GetCurrentHeroAttack => heroInstance.Settings.Attack; // could be altered with armor/items to be more than base attack
        #endregion

        #region Private
        private HeroSettings GetHeroBaseSettings()
        {
            heroConfig.Value.Heroes.TryGetValue(DefaultHeroId, out var heroSettings);
            return heroSettings;
        }

        #endregion
    }
}
