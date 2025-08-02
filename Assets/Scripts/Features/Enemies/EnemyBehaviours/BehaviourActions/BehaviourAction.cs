using Core;
using Core.IoC;
using Features.Heroes;
using System;
using UnityEngine;

namespace Features.Enemies
{
    public enum EnemyBehaviourActionType
    {
        Idle,
        Movement,
        Dash,
        Attack
    }
    
    public class BehaviourAction
    {
        #region Events
        public event Action OnEnter;
        public event Action OnExit;
        #endregion

        #region Dependencies
        [Inject] protected IHeroModel heroModel;
        #endregion

        #region State
        protected IEnemyModel enemyModel;
        protected EnemyBehaviourActionData enemyBehaviourActionData;
        #endregion

        #region Lifecycle
        public void Init(IEnemyModel enemyModel)
        {
            GameContext.Container.ResolveAll(this);
            this.enemyModel = enemyModel;
        }
        
        public virtual void Enter(EnemyBehaviourActionData enemyBehaviourActionData)
        {
            this.enemyBehaviourActionData = enemyBehaviourActionData;
            OnEnter?.Invoke();
            Execute();
        }
        public virtual void Execute() { }

        public virtual void Exit()
        {
            OnExit?.Invoke();
        }
        #endregion
    }
}