using UnityEngine;

namespace Features.Enemies
{
    public class IdleAction : BehaviourAction
    {
        public override void Execute()
        {
            base.Execute();
            enemyModel.EnemyInstance.SetSpeed(0);
            enemyModel.EnemyInstance.PlayIdleAnimation();
        }
    }
}