using Core.IoC;
using DG.Tweening;
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
        public IEnemyModel EnemyModel { get; private set; }
        public Vector3 Position => transform.position;
        #endregion

        #region State
        #endregion

        #region Lifecycle       
        public void SetModel(IEnemyModel enemyModel)
        {
            EnemyModel = enemyModel;
            EnemyModel.OnDamageTaken += OnDamageTaken;
        }
        private void OnDestroy()
        {
            EnemyModel.OnDamageTaken -= OnDamageTaken;
            EnemyModel = null;
        }
        #endregion

        #region Public
        public void MoveTorwardsTarget(Vector3 target)
        {
            navMeshAgent.SetDestination(target);
            animator.SetTrigger("Run");
        }

        public void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
            var deathSequence = DOTween.Sequence();
            deathSequence.AppendInterval(1.5f);
            deathSequence.Append(transform
                .DOScale(Vector3.zero, 3f)
                .SetEase(Ease.OutBounce)
                .OnComplete(() => GameObject.Destroy(gameObject)));
        }
        #endregion

        #region Private
        private void OnDamageTaken(int damage)
        {
            animator.SetTrigger("Take Damage");
        }
        #endregion
    }
}
