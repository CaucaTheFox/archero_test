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
    
    public class AttackEnemyBehaviourAction : BehaviourAction
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

        public override void Execute()
        {
            base.Execute();
            switch (data.SubType)
            {
                case AttackEnemyBehaviourActionSubType.Melee:
                    enemyActor.SetSpeed(0f);
                    enemyActor.PlayIdleAnimation();
                    enemyActor.ExecuteMeleeAttack(GetRandomAttackId());
                    break;
                case AttackEnemyBehaviourActionSubType.Particle:
                    enemyActor.SetSpeed(0f);
                    enemyActor.PlayIdleAnimation();
                    enemyActor.transform.LookAt(heroModel.HeroPosition);
                    enemyActor.ExecuteParticleAttack();
                    break;
                case AttackEnemyBehaviourActionSubType.Ranged:
                    enemyActor.SetSpeed(0f);
                    enemyActor.ExecuteRangedAttack();
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
                    enemyActor.StopParticleAttack();
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