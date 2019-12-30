using Core.IoC;
using Core.Time;
using DG.Tweening;
using UnityEngine;

namespace Features.Enemies
{
    public class DeathKnightBehaviour : EnemyBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private long invisibilityCooldownSeconds;
        [SerializeField] private long invisibilityDuration;
        #endregion

        #region Dependencies
        [Inject] private ITimeProvider timeProvider;
        #endregion

        #region State
        private long lastTimeInvisible;
        private long invisibilityStart;
        #endregion

        #region Lifecycle
        private void Start()
        {
            lastTimeInvisible = timeProvider.TimestampUtcNow; 
        }
        protected override void Update()
        {
            var now = timeProvider.TimestampUtcNow;
            if (enemy.EnemyModel.IsVisible)
            {
                if (now - lastTimeInvisible > invisibilityCooldownSeconds)
                {
                    TurnInvisible();
                    invisibilityStart = now;
                }
            }
            else
            {
                base.Move();
                if (now - invisibilityStart > invisibilityDuration)
                {
                    TurnVisible();
                    lastTimeInvisible = now;
                }
            }
        }
        #endregion

        #region Protected
        protected override void Attack()
        {
            var random = Random.Range(0, 2);
            var anim = random == 0
                ? "Slash Attack 01"
                : "Slash Attack 02";
            enemy.MeleeAttack(anim);
        }
        #endregion

        #region Private
        private void TurnVisible()
        {
            enemy.EnemyModel.IsVisible = true;
            enemy.transform
                .DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutBounce);
            invisibilityStart = 0;
        }

        private void TurnInvisible()
        {
            enemy.EnemyModel.IsVisible = false;
            enemy.transform
                .DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBounce);
        }
        #endregion
    }
}
