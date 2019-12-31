using Core;
using Core.IoC;
using Features.Heroes;
using System;
using UnityEngine;

namespace Features.Enemies
{
    [Serializable]
    public class Behaviour
    {
        public BehaviourAction Action;
        public float ActionDuration;
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
        #endregion

        #region Lifecycle
        public void Init(Enemy enemyActor)
        {
            this.enemyActor = enemyActor;
            GameContext.Container.ResolveAll(this);
        }
        public virtual void Enter()
        {
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