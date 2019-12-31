namespace Features.Enemies
{
    public class BasicMotionAction : BehaviourAction
    {
        public override void Execute()
        {
            base.Execute();
            enemyActor.SetBaseSpeed();
            enemyActor.MoveTorwardsTarget(heroModel.HeroPosition);          
        }
    }
}