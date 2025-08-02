using Core.IoC;
using DG.Tweening;
using System;
using Core;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Enemies
{
    public class Enemy : InjectableBehaviour
    {
        #region Constants
        private const string IdleAnim = "Idle";
        
        private static readonly int DieTrigger = Animator.StringToHash("Die");
        private static readonly int RunTrigger = Animator.StringToHash("Run");
        private static readonly int ParticleAttackTrigger = Animator.StringToHash("ParticleAttack");
        private static readonly int ParticleAttackEndTrigger = Animator.StringToHash("ParticleAttackEnd");
        private static readonly int RangedAttackTrigger = Animator.StringToHash("RangedAttack");
        private static readonly int DamageTrigger = Animator.StringToHash("Take Damage");
        #endregion

        #region Events
        public event Action OnDestinationReached; 
        public event Action<int> OnPlayerHit; 
        #endregion

        #region Unity Serialized Fields
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform healthBarAnchor;
        [SerializeField] private EnemyWeapon collisionWeapon;
        [SerializeField] private EnemyWeapon meleeWeapon;
        [SerializeField] private EnemyWeapon rangedWeapon;
        [SerializeField] private EnemyWeapon particleWeapon;
        [SerializeField] private string[] attackIds;
        #endregion

        #region Properties
        public int InstanceId { get; private set; }
        public Vector3 Position => transform.position;
        public Transform HealthBarAnchor => healthBarAnchor;
        #endregion

        #region State
        private int collisionDamage;
        private int meleeDamage;
        private int particleDamage;
        private int rangedDamage;
        private float baseSpeed;
        private EnemySettings enemySettings;
        #endregion

        #region Lifecycle       
        public void Init(int instanceId, EnemySettings enemySettings)
        {
            InstanceId = instanceId;
            this.enemySettings = enemySettings;
            baseSpeed = navMeshAgent.speed;
            
            if (collisionWeapon != null)
            {
                collisionWeapon.OnPlayerHit += DispatchPlayerCollision;
            }

            if (meleeWeapon != null)
            {
                meleeWeapon.OnPlayerHit += DispatchPlayerMeleeHit;
            }

            if (particleWeapon != null)
            {
                particleWeapon.OnPlayerHit += DispatchPlayerParticleHit;
            }

            if (rangedWeapon != null)
            {
                rangedWeapon.OnPlayerHit += DispatchPlayerRangedHit;
            }
        }

        private void Update()
        {
            if (!navMeshAgent.enabled || navMeshAgent.pathPending || 
                navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                return;

            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                OnDestinationReached?.Invoke();
            }
        }
        #endregion

        #region Public
        public void MoveTowardsTarget(Vector3 target)
        {
            navMeshAgent.SetDestination(target);
            animator.SetTrigger(RunTrigger);
        }

        public void StopAgent()
        {
            navMeshAgent.isStopped = true;
        }
        
        public void PlayDeathAnimation()
        {
            animator.SetTrigger(DieTrigger);
            var deathSequence = DOTween.Sequence();
            deathSequence.AppendInterval(1.5f);
            deathSequence.Append(transform
                .DOScale(Vector3.zero, 1.5f)
                .OnComplete(() => Destroy(gameObject)));
        }

        public void ExecuteMeleeAttack()
        {
            if (meleeWeapon == null)
                return;
            
            var attackTriggerId = GetRandomAttackTriggerId();
            animator.SetTrigger(attackTriggerId);
            meleeWeapon.Attack();
        }
        
        public void ExecuteParticleAttack()
        {
            if (particleWeapon == null)
                return;
            
            animator.SetTrigger(ParticleAttackTrigger);
            particleWeapon.Attack();
        }

        public void StopParticleAttack()
        {
            if (particleWeapon == null)
                return;
            
            animator.SetTrigger(ParticleAttackEndTrigger);
            particleWeapon.HideParticle();
        }

        public void ExecuteRangedAttack()
        {
            if (rangedWeapon == null)
                return;
            
            animator.SetTrigger(RangedAttackTrigger);
            rangedWeapon.Attack();
        }

        public void SetSpeed(float speed)
        {
            navMeshAgent.speed = speed;
            navMeshAgent.isStopped = speed == 0;
        }

        public void SetBaseSpeed()
        {
            navMeshAgent.speed = baseSpeed;
            navMeshAgent.isStopped = false;
        }

        public void PlayIdleAnimation()
        {
            animator.Play(IdleAnim);
        }

        public void PlayDamageAnimation()
        {
            animator.SetTrigger(DamageTrigger);
        }
        
        #endregion

        #region Private
        private string GetRandomAttackTriggerId()
        {
            var index = UnityEngine.Random.Range(0, attackIds.Length);
            return attackIds[index];
        }
        
        private void DispatchPlayerCollision()
        {
            OnPlayerHit?.Invoke(enemySettings.CollisionDamage);
        }

        private void DispatchPlayerMeleeHit()
        {
            OnPlayerHit?.Invoke(enemySettings.MeleeDamage);
        }

        private void DispatchPlayerParticleHit()
        {
            OnPlayerHit?.Invoke(enemySettings.ParticleDamage);
        }

        private void DispatchPlayerRangedHit()
        {
            OnPlayerHit?.Invoke(enemySettings.RangedDamage);
        }
        #endregion
    }
}
