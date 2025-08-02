using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Enemies
{
    public enum MovementEnemyBehaviourActionSubType
    {
        Default,
        Invisible,
        RandomDirection,
        Wave
    }
    
    public class MovementEnemyBehaviourAction : BehaviourAction
    {
        #region State
        private MovementEnemyBehaviourActionData data;
        #endregion
        
        #region Lifecycle

        public override void Enter(EnemyBehaviourActionData enemyBehaviourActionData)
        {
            data = enemyBehaviourActionData as MovementEnemyBehaviourActionData;
            if (data == null)
            {
                throw new Exception("[MovementEnemyBehaviourAction] Invalid EnemyBehaviourActionData");
            }
            
            if (data.SubType == MovementEnemyBehaviourActionSubType.Invisible)
            {
                enemyActor.EnemyModel.IsVisible = false;
                enemyActor.transform
                    .DOScale(Vector3.zero, 0.3f)
                    .SetEase(Ease.InBounce);
            }
            
            base.Enter(enemyBehaviourActionData);
        }

        public override void Execute()
        {
            base.Execute();
            enemyActor.SetBaseSpeed();
            switch (data.SubType)
            {
                case MovementEnemyBehaviourActionSubType.Default:
                case MovementEnemyBehaviourActionSubType.Invisible:
                    enemyActor.MoveTowardsTarget(heroModel.HeroPosition);
                    break;
                case MovementEnemyBehaviourActionSubType.RandomDirection:
                    MoveInRandomDirection();
                    break;
                case MovementEnemyBehaviourActionSubType.Wave:
                    MoveInWaves();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            enemyActor.OnDestinationReached += HandleDestinationReached; 
        }

        public override void Exit()
        {
            enemyActor.OnDestinationReached -= HandleDestinationReached;
            if (data.SubType == MovementEnemyBehaviourActionSubType.Invisible)
            {
                enemyActor.EnemyModel.IsVisible = true;
                enemyActor.transform
                    .DOScale(Vector3.one, 0.3f)
                    .SetEase(Ease.OutBounce);
            }
            base.Exit();
        }
        #endregion
        
        #region Private
        private void HandleDestinationReached()
        {
            if (data.SubType == MovementEnemyBehaviourActionSubType.RandomDirection)
            {
                MoveInRandomDirection();
            }
            else
            {
                enemyActor.MoveTowardsTarget(heroModel.HeroPosition);
            }
        }
        
        private void MoveInRandomDirection()
        {
            var randomDirection = Random.insideUnitSphere * data.RandomDirectionScalar;
            randomDirection.y = 0f;
            enemyActor.MoveTowardsTarget(randomDirection);
        }
        
        private void MoveInWaves()
        {
            var heroPosition = heroModel.HeroPosition;
            heroPosition += enemyActor.transform.right * (Mathf.Sin(Time.time * data.WaveFrequency) * data.WaveMagnitude);
            enemyActor.MoveTowardsTarget(heroPosition);
        }
        #endregion
    }
}