using System;

namespace Features.Enemies
{
    public class DashEnemyBehaviourAction : EnemyBehaviourAction
    {
        #region State
        private DashEnemyBehaviourActionData data;
        #endregion

        #region Lifecycle
        public override void Enter(EnemyBehaviourActionData enemyBehaviourActionData)
        {
            data = enemyBehaviourActionData as DashEnemyBehaviourActionData;
            if (data == null)
            {
                throw new Exception("[DashEnemyBehaviourAction] Invalid EnemyBehaviourActionData");
            }
            
            base.Enter(enemyBehaviourActionData);
        }
        protected override void Execute()
        {
            base.Execute();
            enemyModel.EnemyInstance.SetSpeed(data.Speed);
            enemyModel.EnemyInstance.MoveTowardsTarget(heroModel.HeroPosition);
        }
        #endregion
    }
}