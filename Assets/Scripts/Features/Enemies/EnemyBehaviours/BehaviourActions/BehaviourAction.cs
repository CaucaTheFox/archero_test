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
    
    public class BehaviourAction : MonoBehaviour
    {
        #region Events
        public event Action OnEnter;
        public event Action OnExit;
        #endregion

        #region Dependencies
        [Inject] protected IHeroModel heroModel;
        #endregion

        #region State
        protected Enemy enemyActor;
        protected EnemyBehaviourActionData enemyBehaviourActionData;
        #endregion

        #region Lifecycle
        public void Init(Enemy enemyActor)
        {
            this.enemyActor = enemyActor;
            GameContext.Container.ResolveAll(this);
        }
        public virtual void Enter(EnemyBehaviourActionData enemyBehaviourActionData)
        {
            this.enemyBehaviourActionData = enemyBehaviourActionData;
            OnEnter?.Invoke();
            Execute();
        }
        public virtual void Execute()
        {

        }

        public virtual void Exit()
        {
            OnExit?.Invoke();
        }
        #endregion
    }
}