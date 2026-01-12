using System;
using System.Collections.Generic;
using System.Linq;
using Core.IoC;

namespace Features.Enemies
{
    public interface IEnemiesModel
    {
        event Action OnDeath;
        event Action<int> OnPlayerHit;
        
        Dictionary<int, EnemyModel> EnemyModels { get; }
        
        void InitModel();
        void Cleanup();
        
        void ApplyDamage(int instanceId, int damage);
        void SpawnEnemyWave();
    }

    public class EnemiesModel : IEnemiesModel
    {
        #region Constants
        private const int MaxEnemies = 4; // for demo purpose only, enemy count per room would be defined through configs usually
        #endregion

        #region Events
        public event Action<IEnemyModel> OnDamageTaken;
        public event Action OnDeath;
        public event Action<int> OnPlayerHit;
        #endregion

        #region Dependencies
        [Inject] private IJsonConfig<EnemyConfig> enemyConfig;
        #endregion

        #region Properties
        public Dictionary<int, EnemyModel> EnemyModels { get; private set; }
        #endregion
        
        #region Lifecycle
        public void InitModel()
        {
            SpawnEnemyWave();
        }

        public void Cleanup()
        {
            foreach (var entry in EnemyModels)
            {
                entry.Value.Cleanup();
                entry.Value.OnDeath -= DispatchDeath;
                entry.Value.OnPlayerHit -= DispatchPlayerHit;
            }
            EnemyModels.Clear();
        }
        #endregion
        
        #region Public
        public void ApplyDamage(int instanceId, int damage)
        {
            if (EnemyModels.TryGetValue(instanceId, out var enemyModel))
            {
                enemyModel.ApplyDamage(damage);
            }
        }

        public void SpawnEnemyWave()
        {
            var enemyAmount = UnityEngine.Random.Range(1, MaxEnemies);
            GenerateEnemies(enemyAmount);
        }
        #endregion

        #region Private
        private void GenerateEnemies(int enemyAmount)
        {
            EnemyModels = new Dictionary<int, EnemyModel>();
            var enemySettings = enemyConfig.Value.Enemies.Select(x => x.Value).ToList();
            for (int i = 0; i < enemyAmount; i++)
            {
                var instanceId = i;
                var randomIndex = UnityEngine.Random.Range(0, enemyConfig.Value.Enemies.Count);
                var randomEnemySettings = enemySettings[randomIndex];
                var enemyModel = new EnemyModel(instanceId, randomEnemySettings);
                enemyModel.OnDeath += DispatchDeath;
                enemyModel.OnPlayerHit += DispatchPlayerHit;
                EnemyModels.Add(instanceId, enemyModel);
            }
        }

        private void RemoveEnemyModel(int instanceId)
        {
            if (EnemyModels.TryGetValue(instanceId, out var enemyModel))
            {
                enemyModel.Cleanup();
                enemyModel.OnDeath -= DispatchDeath;
                enemyModel.OnPlayerHit -= DispatchPlayerHit;
            }
            EnemyModels.Remove(instanceId);
        }
        
        private void DispatchDeath(int instanceId)
        {
            RemoveEnemyModel(instanceId);
            OnDeath?.Invoke();
        }

        private void DispatchPlayerHit(int damage)
        {
            OnPlayerHit?.Invoke(damage);
        }
        #endregion
    }
}
