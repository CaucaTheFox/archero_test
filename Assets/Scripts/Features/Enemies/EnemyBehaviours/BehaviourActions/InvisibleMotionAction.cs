using DG.Tweening;
using UnityEngine;

namespace Features.Enemies
{
    public class InvisibleMotionAction : BehaviourAction
    {
        public override void Enter()
        {
            enemyActor.EnemyModel.IsVisible = true;
            enemyActor.transform
                .DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBounce);
            base.Enter();
        }
        public override void Execute()
        {
            base.Execute();
            enemyActor.SetBaseSpeed();
            enemyActor.MoveTorwardsTarget(heroModel.HeroPosition);
            enemyActor.OnDestinationReached += HandleDestinationReached;
        }

        public override void Exit()
        {
            enemyActor.OnDestinationReached -= HandleDestinationReached;
            enemyActor.EnemyModel.IsVisible = false;
            enemyActor.transform
                .DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutBounce);
            base.Exit();
        }

        private void HandleDestinationReached()
        {
            enemyActor.MoveTorwardsTarget(heroModel.HeroPosition);
        }
    }
}