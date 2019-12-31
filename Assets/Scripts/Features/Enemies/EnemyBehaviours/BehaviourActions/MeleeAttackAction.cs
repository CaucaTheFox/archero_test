namespace Features.Enemies
{
    public class MeleeAttackAction : BehaviourAction
    {
        public override void Execute()
        {
            base.Execute();
            enemyActor.SetSpeed(0f);
            enemyActor.MeleeAttack();
        }
    }
}