using Core.IoC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Features.Enemies
{
    public interface IEnemiesModel : IEnumerable<IEnemyModel>
    {
        void Init();
    }

    public class EnemiesModel : IEnemiesModel
    {
        #region Constants
        private const int MaxEnemies = 4; // for demo purpose, enemy count per room would be defined by GD differently 
        #endregion
        #region Dependencies
        [Inject] private IJsonConfig<EnemyConfig> enemyConfig;
        #endregion

        #region State
        private List<EnemyModel> EnemyModels; 
        #endregion

        #region Public
        public void Init()
        {
            EnemyModels = new List<EnemyModel>();
            var enemyAmount = UnityEngine.Random.Range(1, MaxEnemies);
            var enemySettings = enemyConfig.Value.Enemies.Select(x => x.Value).ToList(); 
            for (int i = 0; i < enemyAmount; i++)
            {
                var randomIndex = UnityEngine.Random.Range(0, enemyConfig.Value.Enemies.Count);
                var enemyModel = new EnemyModel(enemySettings[randomIndex]);
                EnemyModels.Add(enemyModel);
            }
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
