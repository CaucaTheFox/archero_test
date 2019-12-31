using UnityEngine;

namespace Features.Enemies
{
    public class WaveMotionAction : BehaviourAction
    {
        #region Unity Serialized Fields
        [SerializeField] private float frequency, magnitude;
        #endregion

        public override void Execute()
        {
            base.Execute();
            enemyActor.SetBaseSpeed();
            var heroPosition = heroModel.HeroPosition;
            heroPosition += enemyActor.transform.right * Mathf.Sin(Time.time * frequency) * magnitude;
            enemyActor.MoveTorwardsTarget(heroPosition);
        }
    }
}