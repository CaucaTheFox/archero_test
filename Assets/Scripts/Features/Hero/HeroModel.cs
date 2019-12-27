using Core.IoC;
using Core.ResourceManagement;
using UnityEngine;

namespace Features.Heroes
{
    public interface IHeroModel
    {
        Vector3 HeroPosition { get; }
        float CurrentHealthNormalized { get; }
        int CurrentHealth { get; }
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
        public Vector3 HeroPosition => heroInstance.Position;
        public float CurrentHealthNormalized => (float) CurrentHealth / heroInstance.Settings.Health;
        public int CurrentHealth { get; set; }

        public int GetCurrentHeroAttack => heroInstance.Settings.Attack; // could be altered with armor/items to be more than base attack
        #endregion

        #region State
        private HeroState currentState;
        private Hero heroInstance;
        #endregion
        #region Public
        public Hero CreateHero(Transform parent)
        {
            var heroTemplate = resourceManager.LoadResource<Hero>(DefaultHeroPath);
            var heroSettings = GetHeroBaseSettings();
            CurrentHealth = heroSettings.Health;
            heroInstance = Object.Instantiate(heroTemplate, parent);
            heroInstance.Settings = heroSettings;
            return heroInstance;
        }
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
