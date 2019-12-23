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
        #endregion

        #region Dependencies
        [Inject] private ITimeProvider timeProvider;
        #endregion

        #region State
        private long lastTimeInvisible;
        private long invisibilityStart;
        private bool isInvisible;
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
            if (isInvisible)
            {
                if (now - invisibilityStart > invisibilityDuration)
                {
                    invisibilityStart = 0;
                    lastTimeInvisible = now;
                    isInvisible = false;
                    enemy.transform
                    .DOScale(Vector3.one, 0.3f)
                    .SetEase(Ease.OutBounce);
                }
                return;
            }

            if (now - lastTimeInvisible > invisibilityCooldownSeconds)
            {
                invisibilityStart = now;
                isInvisible = true;
                enemy.transform
                    .DOScale(Vector3.zero, 0.3f)
                    .SetEase(Ease.InBounce);
            }
        }
        #endregion

        #region Public
        #endregion
    }
}
