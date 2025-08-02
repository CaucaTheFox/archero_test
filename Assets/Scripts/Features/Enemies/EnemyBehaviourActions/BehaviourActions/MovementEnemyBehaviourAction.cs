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
    
    public class MovementEnemyBehaviourAction : EnemyBehaviourAction
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
                enemyModel.IsVisible = false;
                enemyModel.EnemyInstance.transform
                    .DOScale(Vector3.zero, 0.3f)
                    .SetEase(Ease.InBounce);
            }
            
            base.Enter(enemyBehaviourActionData);
        }

        protected override void Execute()
        {
            base.Execute();
            enemyModel.EnemyInstance.SetBaseSpeed();
            switch (data.SubType)
            {
                case MovementEnemyBehaviourActionSubType.Default:
                case MovementEnemyBehaviourActionSubType.Invisible:
                    enemyModel.EnemyInstance.MoveTowardsTarget(heroModel.HeroPosition);
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
            enemyModel.EnemyInstance.OnDestinationReached += HandleDestinationReached; 
        }

        public override void Exit()
        {
            enemyModel.EnemyInstance.OnDestinationReached -= HandleDestinationReached;
            if (data.SubType == MovementEnemyBehaviourActionSubType.Invisible)
            {
                enemyModel.IsVisible = true;
                enemyModel.EnemyInstance.transform
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
                enemyModel.EnemyInstance.MoveTowardsTarget(heroModel.HeroPosition);
            }
        }
        
        private void MoveInRandomDirection()
        {
            var randomDirection = Random.insideUnitSphere * data.RandomDirectionScalar;
            randomDirection.y = 0f;
            enemyModel.EnemyInstance.MoveTowardsTarget(randomDirection);
        }
        
        private void MoveInWaves()
        {
            var heroPosition = heroModel.HeroPosition;
            heroPosition += enemyModel.EnemyInstance.transform.right * (Mathf.Sin(Time.time * data.WaveFrequency) * data.WaveMagnitude);
            enemyModel.EnemyInstance.MoveTowardsTarget(heroPosition);
        }
        #endregion
    }
}