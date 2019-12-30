using UnityEngine;

namespace Features.Enemies
{
    public class BatBehaviour : EnemyBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private float frequency, magnitude, dashChance, dashForce, attackChance;
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
            if (random <= dashChance)
            {
                DashAttack();
                Debug.Log("DASH");
                return;
            }

            //if (random <= attackChance)
            //{
            //    SoundWaveAttack();
            //    MoveToTargetInWaves();
            //    Debug.Log("SOUNDWAVE");
            //    return;
            //}

            MoveToTargetInWaves(); 
        }
        #endregion

        #region Private
        private void MoveToTargetInWaves()
        {
            var heroPosition = heroModel.HeroPosition;
            heroPosition += transform.right * Mathf.Sin(Time.time * frequency) * magnitude;
            enemy.MoveTorwardsTarget(heroPosition);
        }

        private void DashAttack()
        {
            enemy.Rigidbody.isKinematic = false;
            enemy.Rigidbody.AddForce(heroModel.HeroPosition * dashForce, ForceMode.Impulse);
            enemy.Rigidbody.isKinematic = true;
        }


        private void SoundWaveAttack()
        {
            enemy.ParticleAttack("Soundwave Attack");
        }
        #endregion
    }
}
