using System;

namespace Features.Enemies
{
    public interface IEnemyModel
    {
        event Action<int> OnDamageTaken;
        event Action<int> OnDeath;
        event Action<int> OnPlayerHit;
        event Action OnVisibilityChange;

        int Index { get; }
        float CurrentHealthNormalized { get; }
        bool IsDead { get; }
        bool IsVisible { get; set; }
        EnemySettings Settings { get; }
        EnemyState EnemyState { get; set; }
        void ApplyDamage(int damage);
        void DispatchPlayerHit(int damage);

    }
    public enum EnemyState
    {
        Moving,
        MeleeAttack,
        ParticleAttack,
        RangedAttack,
        Dead,
    }

    public class EnemyModel : IEnemyModel
    {
        #region Events
        public event Action<int> OnDamageTaken;
        public event Action<int> OnDeath;
        public event Action<int> OnPlayerHit;
        public event Action OnVisibilityChange;
        #endregion

        #region Properties
        public int Index { get; }
        public float CurrentHealthNormalized => (float) currentHealth / Settings.Health;
        public bool IsDead => EnemyState == EnemyState.Dead;
        public bool IsVisible {
            get => isVisible;
            set
            {
                isVisible = value;
                OnVisibilityChange?.Invoke();
            }
        }

        public EnemySettings Settings { get; }

        public EnemyState EnemyState { get; set; }
        #endregion

        #region State     
        private bool isVisible;
        // in real game setting, these would be applied to a gamestate saved to disk
        private int currentHealth;
        #endregion

        #region Public
        public EnemyModel(EnemySettings settings, int index)
        {
            this.Settings = settings;
            this.Index = index;
            currentHealth = settings.Health;
            EnemyState = EnemyState.Moving;
            isVisible = true;
        }

        public void ApplyDamage(int damage)
        {
            if (EnemyState == EnemyState.Dead || !IsVisible)
            {
                return;
            }

            currentHealth -= damage + Settings.DamageResistance; 
            if (currentHealth > 0)
            {
                OnDamageTaken?.Invoke(Index);
            }
            else
            {
                EnemyState = EnemyState.Dead;
                OnDeath?.Invoke(Index);
            }
        }

        public void DispatchPlayerHit(int damage)
        {
            OnPlayerHit.Invoke(damage);
    }
        #endregion
    }
}
