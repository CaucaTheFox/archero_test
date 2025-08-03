using System;
using UnityEngine;

namespace Features.Enemies
{
    [Serializable]
    public class EnemyBehaviourActionData
    {
        public float MinActionDuration, MaxActionDuration;
        
        public virtual EnemyBehaviourActionType GetEnemyBehaviourActionType( )
        {
            return EnemyBehaviourActionType.Idle; 
        }
    }
    
    [Serializable]
    public class IdleEnemyBehaviourActionData : EnemyBehaviourActionData
    {
        [HideInInspector] public EnemyBehaviourActionType Type = EnemyBehaviourActionType.Idle;
        
        public override EnemyBehaviourActionType GetEnemyBehaviourActionType()
        {
            return Type; 
        }
    }

    [Serializable]
    public class MovementEnemyBehaviourActionData : EnemyBehaviourActionData
    {
        [HideInInspector] public EnemyBehaviourActionType Type = EnemyBehaviourActionType.Movement;
        public MovementEnemyBehaviourActionSubType SubType;
        public float RandomDirectionScalar;
        public float WaveFrequency, WaveMagnitude;
        
        public override EnemyBehaviourActionType GetEnemyBehaviourActionType()
        {
            return Type; 
        }
    }
    
    [Serializable]
    public class DashEnemyBehaviourActionData : EnemyBehaviourActionData
    {
        [HideInInspector] public EnemyBehaviourActionType Type = EnemyBehaviourActionType.Dash;
        public float Speed;
        
        public override EnemyBehaviourActionType GetEnemyBehaviourActionType()
        {
            return Type; 
        }
    }
    
    [Serializable]
    public class AttackEnemyBehaviourActionData : EnemyBehaviourActionData
    {
        [HideInInspector] public EnemyBehaviourActionType Type = EnemyBehaviourActionType.Attack;
        public AttackEnemyBehaviourActionSubType SubType;
        
        public override EnemyBehaviourActionType GetEnemyBehaviourActionType()
        {
            return Type; 
        }
    }
}
