namespace Features.Enemies
{
    public class BasicMotionAction : BehaviourAction
    {
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
            base.Exit();
        }
        private void HandleDestinationReached()
        {
            enemyActor.MoveTorwardsTarget(heroModel.HeroPosition);
        }
    }
}