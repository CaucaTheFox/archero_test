using System;

namespace Features.Enemies
{
    public interface IEnemyModel
    {
        event Action<int> OnDamageTaken;
        event Action<int> OnDeath;
        event Action OnVisibilityChange;

        int Index { get; }
        float CurrentHealthNormalized { get; }
        bool IsDead { get; }
        bool IsVisible { get; set; }
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
        public event Action OnVisibilityChange;
        #endregion

        #region Properties
        public int Index { get; }
        public float CurrentHealthNormalized => (float) currentHealth / settings.Health;
        public bool IsDead => state == EnemyState.Dead;
        public bool IsVisible {
            get => isVisible;
            set
            {
                isVisible = value;
                OnVisibilityChange?.Invoke();
            }
        }
        #endregion

        #region State     
        private EnemySettings settings;
        private bool isVisible;
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
            isVisible = true;
        }

        public EnemySettings GetSettings()
        {
            return settings;
        }
        public void ApplyDamage(int damage)
        {
            if (state == EnemyState.Dead || !IsVisible)
            {
                return;
            }

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
