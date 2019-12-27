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
        [SerializeField] private Transform healthBarAnchor;
        #endregion

        #region Dependencies
        #endregion

        #region Properties
        public IEnemyModel EnemyModel { get; private set; }
        public Vector3 Position => transform.position;
        public Transform HealthBarAnchor => healthBarAnchor;
        #endregion

        #region State
        private float baseSpeed;
        #endregion

        #region Lifecycle       
        private void Start()
        {
            baseSpeed = navMeshAgent.speed;
        }
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
            if (EnemyModel.IsDead)
            {
                return;
            }
            navMeshAgent.SetDestination(target);
            var trigger = navMeshAgent.speed > 3
                ? "Run"
                : "Walk";

            animator.SetTrigger(trigger);
        }

        public void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
            var deathSequence = DOTween.Sequence();
            deathSequence.AppendInterval(1.5f);
            deathSequence.Append(transform
                .DOScale(Vector3.zero, 1.5f)
                .OnComplete(() => Destroy(gameObject)));
        }

        public void ChangeSpeed(float speed)
        {
            navMeshAgent.speed = speed;
        }
        public void ResetSpeed()
        {
            navMeshAgent.speed = baseSpeed;
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
