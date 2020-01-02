using Core.IoC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Features.Enemies
{
    public interface IEnemiesModel : IEnumerable<IEnemyModel>
    {
        event Action<IEnemyModel> OnDamageTaken;
        event Action<IEnemyModel> OnDeath;
        event Action<int> OnPlayerHit;
        void Init();
        void ApplyDamage(int enemyIndex, int damage);
        void GenerateNextWave();
    }

    public class EnemiesModel : IEnemiesModel
    {
        #region Constants
        private const int MaxEnemies = 4; // for demo purpose, enemy count per room would be defined by GD differently 
        #endregion

        #region Events
        public event Action<IEnemyModel> OnDamageTaken;
        public event Action<IEnemyModel> OnDeath;
        public event Action<int> OnPlayerHit;
        #endregion

        #region Dependencies
        [Inject] private IJsonConfig<EnemyConfig> enemyConfig;
        #endregion

        #region State
        private EnemyModel[] EnemyModels; 
        #endregion

        #region Public
        public void Init()
        {
            var enemyAmount = UnityEngine.Random.Range(1, MaxEnemies);
            GenerateEnemies(enemyAmount);
        }

        public void ApplyDamage(int enemyIndex, int damage)
        {
            if (enemyIndex >= EnemyModels.Length)
            {
                return;
            }
            EnemyModels[enemyIndex].ApplyDamage(damage);
        }

        public void GenerateNextWave()
        {
            foreach (var model in EnemyModels)
            {
                model.OnDamageTaken -= DispatchDamageTaken;
                model.OnDeath -= DispatchDamageTaken;
                model.OnPlayerHit -= DispatchPlayerHit;
            }

            var enemyAmount = UnityEngine.Random.Range(1, MaxEnemies);
            GenerateEnemies(enemyAmount);
        }
        #endregion

        #region Private
        private void GenerateEnemies(int enemyAmount)
        {
            EnemyModels = new EnemyModel[enemyAmount];
            var enemySettings = enemyConfig.Value.Enemies.Select(x => x.Value).ToList();
            for (int i = 0; i < enemyAmount; i++)
            {
                var randomIndex = UnityEngine.Random.Range(0, enemyConfig.Value.Enemies.Count);
                var enemyModel = new EnemyModel(enemySettings[randomIndex], i);
                EnemyModels[i] = enemyModel;
                enemyModel.OnDamageTaken += DispatchDamageTaken;
                enemyModel.OnDeath += DispatchDeath;
                enemyModel.OnPlayerHit += DispatchPlayerHit;
            }
        }

        private void DispatchDamageTaken(int enemyIndex)
        {
            OnDamageTaken?.Invoke(EnemyModels[enemyIndex]);
        }

        private void DispatchDeath(int enemyIndex)
        {
            OnDeath?.Invoke(EnemyModels[enemyIndex]);
        }

        private void DispatchPlayerHit(int damage)
        {
            OnPlayerHit?.Invoke(damage);
        }

        #endregion

        #region Enumerable
        public IEnumerator<IEnemyModel> GetEnumerator()
        {
            foreach (var enemy in EnemyModels)
            {
                yield return enemy;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
