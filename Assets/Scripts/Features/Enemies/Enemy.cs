using Core.IoC;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Enemies
{
    public class Enemy : InjectableBehaviour
    {
        #region Constants
        private const string DeathAnim = "Die";
        private const string RunAnim = "Run";
        private const string MeleeAttackAnim = "MeleeAttack";
        private const string ParticleAttackAnim = "ParticleAttack";
        private const string RangedAttackAnim = "RangedAttack";
        private const string DamageAnim = "Take Damage";
        private const string IdleAnim = "Idle";
        #endregion

        #region Events
        public event Action OnDestinationReached; 
        #endregion

        #region Unity Serialized Fields
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform healthBarAnchor;
        [SerializeField] private EnemyWeapon meleeWeapon;
        [SerializeField] private EnemyWeapon rangedWeapon;
        [SerializeField] private EnemyWeapon particleWeapon;
        #endregion

        #region Properties
        public IEnemyModel EnemyModel { get; private set; }
        public Vector3 Position => transform.position;
        public Transform HealthBarAnchor => healthBarAnchor;
        #endregion

        #region State
        private int meleeDamage;
        private int particleDamage;
        private int rangedDamage;
        private float baseSpeed;
        #endregion

        #region Lifecycle       
        public void Init(IEnemyModel enemyModel)
        {
            EnemyModel = enemyModel;
            EnemyModel.OnDamageTaken += OnDamageTaken;
            if (meleeWeapon != null)
            {
                meleeDamage = EnemyModel.Settings.MeleeDamage;
                meleeWeapon.OnPlayerHit += HandleHitPlayerMelee;
            }

            if (particleWeapon != null)
            {
                particleDamage = EnemyModel.Settings.ParticleDamage;
                particleWeapon.OnPlayerHit += HandleHitPlayerParticle;
            }

            if (rangedWeapon != null)
            {
                rangedDamage = EnemyModel.Settings.RangedDamage;
                rangedWeapon.OnPlayerHit += HandleHitPlayerRanged;
            }

            baseSpeed = navMeshAgent.speed;
        }

        private void Update()
        {
            if (
                !navMeshAgent.enabled
                || navMeshAgent.pathPending
                || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance
            )
            {
                return;
            }

            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                OnDestinationReached?.Invoke();
            }
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
            animator.SetTrigger(RunAnim);
        }

        public void StopAgent()
        {
            navMeshAgent.isStopped = true;
        }
        public void PlayDeathAnimation()
        {
            animator.SetTrigger(DeathAnim);
            var deathSequence = DOTween.Sequence();
            deathSequence.AppendInterval(1.5f);
            deathSequence.Append(transform
                .DOScale(Vector3.zero, 1.5f)
                .OnComplete(() => Destroy(gameObject)));
        }

        public void MeleeAttack()
        {
            if (meleeWeapon == null)
            {
                throw new System.Exception($"[Enemy] {EnemyModel.Settings.Id} does not have a melee weapon");
            }
            animator.SetTrigger(MeleeAttackAnim);
            meleeWeapon.Attack();
        }
        public void ParticleAttack()
        {
            if (particleWeapon == null)
            {
                throw new System.Exception($"[Enemy] {EnemyModel.Settings.Id} does not have a particle weapon");
            }
            animator.SetTrigger(ParticleAttackAnim);
            particleWeapon.Attack();
        }

        public void EndParticleAttack()
        {
            particleWeapon.HideParticle();
        }

        public void RangedAttack()
        {
            if (rangedWeapon == null)
            {
                throw new System.Exception($"[Enemy] {EnemyModel.Settings.Id} does not have a ranged weapon");
            }
            animator.SetTrigger(RangedAttackAnim);
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
        #endregion

        #region Private
        private void OnDamageTaken(int damage)
        {
            animator.SetTrigger(DamageAnim);
        }
        private void HandleHitPlayerMelee()
        {
            EnemyModel.DispatchPlayerHit(meleeDamage);
        }

        private void HandleHitPlayerParticle()
        {
            EnemyModel.DispatchPlayerHit(particleDamage);
        }

        private void HandleHitPlayerRanged()
        {
            EnemyModel.DispatchPlayerHit(rangedDamage);
        }
        #endregion
    }
}
