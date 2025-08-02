using System;
using Random = UnityEngine.Random;

namespace Features.Enemies
{
    public enum AttackEnemyBehaviourActionSubType
    {
        Melee,
        Particle,
        Ranged
    }
    
    public class AttackEnemyBehaviourAction : EnemyBehaviourAction
    {
        #region State
        private AttackEnemyBehaviourActionData data;
        #endregion
        
        #region Lifecycle

        public override void Enter(EnemyBehaviourActionData enemyBehaviourActionData)
        {
            data = enemyBehaviourActionData as AttackEnemyBehaviourActionData;
            if (data == null)
            {
                throw new Exception("[AttackEnemyBehaviourAction] Invalid EnemyBehaviourActionData");
            }
            
            base.Enter(enemyBehaviourActionData);
        }

        protected override void Execute()
        {
            base.Execute();
            enemyModel.EnemyInstance.SetSpeed(0f);
            switch (data.SubType)
            {
                case AttackEnemyBehaviourActionSubType.Melee:
                    enemyModel.EnemyInstance.PlayIdleAnimation();
                    enemyModel.EnemyInstance.ExecuteMeleeAttack(GetRandomAttackId());
                    break;
                case AttackEnemyBehaviourActionSubType.Particle:
                    enemyModel.EnemyInstance.PlayIdleAnimation();
                    enemyModel.EnemyInstance.transform.LookAt(heroModel.HeroPosition);
                    enemyModel.EnemyInstance.ExecuteParticleAttack();
                    break;
                case AttackEnemyBehaviourActionSubType.Ranged:
                    enemyModel.EnemyInstance.ExecuteRangedAttack();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
          
        }

        public override void Exit()
        {
            base.Exit();
            switch (data.SubType)
            {
                case AttackEnemyBehaviourActionSubType.Melee:
                    break;
                case AttackEnemyBehaviourActionSubType.Particle:
                    enemyModel.EnemyInstance.StopParticleAttack();
                    break;
                case AttackEnemyBehaviourActionSubType.Ranged:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion
        
        #region Private
        private string GetRandomAttackId()
        {
            var randomIndex = Random.Range(0, data.AttackIds.Length);
            return data.AttackIds[randomIndex];
        }
        #endregion
    }
}