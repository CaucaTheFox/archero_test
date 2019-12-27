using System;
using UnityEngine;

namespace Features.Enemies
{
    public interface IEnemyModel
    {
        event Action<int> OnDamageTaken;
        event Action<int> OnDeath; 

        int Index { get; }
        EnemySettings GetSettings();
        void ApplyDamage(int damage);
    }
    public enum EnemyState
    {
        Alive,
        Dead,
    }

    public class EnemyModel : IEnemyModel
    {
        #region Events
        public event Action<int> OnDamageTaken;
        public event Action<int> OnDeath;
        #endregion

        #region Properties
        public int Index { get; }
        #endregion

        #region State     
        private EnemySettings settings;
        // in real game setting, these would be applied to a gamestate saved to disk
        private int currentHealth;
        private EnemyState state; 
        #endregion

        #region Public
        public EnemyModel(EnemySettings settings, int index)
        {
            this.settings = settings;
            this.Index = index;
            currentHealth = settings.Health;
            state = EnemyState.Alive;
        }

        public EnemySettings GetSettings()
        {
            return settings;
        }
        public void ApplyDamage(int damage)
        {
            currentHealth -= damage + settings.DamageResistance; 
            if (currentHealth > 0)
            {
                OnDamageTaken?.Invoke(Index);
            }
            else
            {
                state = EnemyState.Dead;
                OnDeath?.Invoke(Index);
            }
        }
        #endregion
    }
}
