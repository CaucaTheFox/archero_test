using Core.IoC;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Enemies
{
    public class Enemy : InjectableBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        #endregion

        #region Dependencies
        #endregion

        #region Properties
        public IEnemyModel EnemyModel {get;set;}
        #endregion

        #region Lifecycle
        #endregion

        #region Public
        public void MoveTorwardsTarget
            (Vector3 target)
        {
            navMeshAgent.SetDestination(target);
            animator.SetTrigger("Run");
        }

        public void Shoot()
        {
            animator.SetTrigger("Idle");
            animator.SetTrigger("Arrow Attack");
        }
        #endregion

        #region Private
        #endregion
    }
}
