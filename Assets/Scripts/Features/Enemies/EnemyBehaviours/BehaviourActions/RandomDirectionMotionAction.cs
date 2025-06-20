using UnityEngine;

namespace Features.Enemies
{
    public class RandomDirectionMotionAction : BehaviourAction
    {
        [SerializeField] private float directionScalar;

        public override void Execute()
        {
            base.Execute();
            enemyActor.SetBaseSpeed();
            MoveInRandomDirection();
            enemyActor.OnDestinationReached += HandleDestinationReached;
        }

        public override void Exit()
        {
            enemyActor.OnDestinationReached -= HandleDestinationReached;
            base.Exit();
        }
        private void HandleDestinationReached()
        {
            MoveInRandomDirection();
        }

        private void MoveInRandomDirection()
        {
            var randomDirection = Random.insideUnitSphere * directionScalar;
            randomDirection.y = 0f;
            enemyActor.MoveTowardsTarget(randomDirection);
        }
    }
}