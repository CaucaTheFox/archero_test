using UnityEngine;

namespace Features.Enemies
{
    public class DragonBehaviour : EnemyBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private float attackChance;
        #endregion

        #region Dependencies
        #endregion

        #region State
        #endregion

        #region Lifecycle
        protected override void Update()
        {
            if (enemy.EnemyModel.EnemyState != EnemyState.Moving)
            {
                return;
            }

            var random = Random.value; 
            if (random <= attackChance)
            {
                BreathAttack();
                return;
            }

            MoveTowardsHero(); 
        }
        #endregion

        #region Private
        private void MoveTowardsHero()
        {
            enemy.MoveTorwardsTarget(heroModel.HeroPosition * Time.deltaTime);
        }

        private void BreathAttack()
        {
            enemy.ParticleAttack("Fire Breath Attack");
        }
        #endregion
    }
}
