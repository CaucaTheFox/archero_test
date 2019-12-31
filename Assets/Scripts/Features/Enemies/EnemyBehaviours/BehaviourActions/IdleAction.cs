using UnityEngine;

namespace Features.Enemies
{
    public class IdleAction : BehaviourAction
    {
        public override void Execute()
        {
            base.Execute();
            enemyActor.SetSpeed(0);
            enemyActor.PlayIdleAnimation();
        }
    }
}