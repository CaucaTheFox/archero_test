using Core.IoC;
using Core.ResourceManagement;
using System; 
using UnityEngine;

namespace Features.Heroes
{
    public interface IHeroModel
    {
        event Action OnDamageTaken;
        event Action OnDodge;
        event Action OnDeath;

        Vector3 HeroPosition { get; }
        Vector3 HeroForward { get; }
        float CurrentHealthNormalized { get; }
        int CurrentHealth { get; }
        int GetCurrentHeroAttack { get; }
        HeroState CurrentState { get;}
        Hero CreateHero(Transform parent);
        void ApplyDamage(int damage);
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

        #region Events
        public event Action OnDamageTaken;
        public event Action OnDodge;
        public event Action OnDeath;
        #endregion

        #region Dependencies
        [Inject] private IJsonConfig<HeroConfig> heroConfig;
        [Inject] private IResourceManager resourceManager;
        #endregion

        #region Properties
        public Vector3 HeroPosition => heroInstance.Position;
        public Vector3 HeroForward => heroInstance.transform.forward;
        public float CurrentHealthNormalized => (float) CurrentHealth / heroInstance.Settings.Health;
        public int CurrentHealth { get; set; }
        public HeroState CurrentState { get; set; }

        public int GetCurrentHeroAttack => heroInstance.Settings.Attack; // could be altered with armor/items to be more than base attack
        #endregion

        #region State
        private Hero heroInstance;
        #endregion
        #region Public
        public Hero CreateHero(Transform parent)
        {
            var heroTemplate = resourceManager.LoadResource<Hero>(DefaultHeroPath);
            var heroSettings = GetHeroBaseSettings();
            CurrentHealth = heroSettings.Health;
            heroInstance = UnityEngine.Object.Instantiate(heroTemplate, parent);
            heroInstance.Settings = heroSettings;
            CurrentState = HeroState.Alive;
            return heroInstance;
        }

        public void ApplyDamage(int damage)
        {
            var random = UnityEngine.Random.value;
            if (random <= heroInstance.Settings.Dodge)
            {
                OnDodge?.Invoke();
                return;
            }

            CurrentHealth -= damage;
            if (CurrentHealth > 0)
            {
                heroInstance.PlayDamageAnimation();
                OnDamageTaken?.Invoke();
            }
            else
            {
                CurrentState = HeroState.Dead;
                heroInstance.PlayDeathAnimation();
                OnDeath?.Invoke();
            }
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
