using System;
using DG.Tweening;
using UnityEngine;

namespace Features.Enemies
{
    public class IdleEnemyBehaviourAction : EnemyBehaviourAction
    {
        #region State
        private IdleEnemyBehaviourActionData data;
        #endregion
        
        #region Lifecycle
        public override void Enter(EnemyBehaviourActionData enemyBehaviourActionData)
        {
            data = enemyBehaviourActionData as IdleEnemyBehaviourActionData;
            if (data == null)
            {
                throw new Exception("[IdleEnemyBehaviourAction] Invalid EnemyBehaviourActionData");
            }
            
            base.Enter(enemyBehaviourActionData);
        }

        protected override void Execute()
        {
            base.Execute();
            enemyModel.EnemyInstance.SetSpeed(0);
            enemyModel.EnemyInstance.PlayIdleAnimation();
        }
        #endregion
    }
}