using Core.IoC;
using Core.Time;
using DG.Tweening;
using Features.Heroes;
using UnityEngine;

namespace Features.Enemies
{
    public class SinusoidalBehaviour : EnemyBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private float frequency, magnitude;
        #endregion

        #region Dependencies
        #endregion

        #region State
        #endregion

        #region Lifecycle
        private void Start()
        {
        }
        public override void Update()
        {
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
        #endregion
    }
}
