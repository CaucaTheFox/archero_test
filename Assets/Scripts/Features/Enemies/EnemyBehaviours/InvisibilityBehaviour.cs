using Core.IoC;
using Core.Time;
using DG.Tweening;
using UnityEngine;

namespace Features.Enemies
{
    public class InvisibilityBehaviour : EnemyBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private long invisibilityCooldownSeconds;
        [SerializeField] private long invisibilityDuration;
        [SerializeField] private float invisibilitySpeed;
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
        public override void Update()
        {
            base.Update();

            var now = timeProvider.TimestampUtcNow;
            if (!enemy.EnemyModel.IsVisible)
            {
                if (now - invisibilityStart > invisibilityDuration)
                {
                    enemy.EnemyModel.IsVisible = true;
                    enemy.transform
                        .DOScale(Vector3.one, 0.3f)
                        .SetEase(Ease.OutBounce);
                    enemy.ResetSpeed();
                    invisibilityStart = 0;
                    lastTimeInvisible = now;
                }
                return;
            }

            if (now - lastTimeInvisible > invisibilityCooldownSeconds)
            {
                enemy.EnemyModel.IsVisible = false;
                enemy.transform
                    .DOScale(Vector3.zero, 0.3f)
                    .SetEase(Ease.InBounce);
                enemy.ChangeSpeed(invisibilitySpeed);
                invisibilityStart = now;
            }
        }
        #endregion

        #region Public
        #endregion
    }
}
