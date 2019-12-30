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
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private Transform healthBarAnchor;
        [SerializeField] private EnemyWeapon meleeWeapon;
        [SerializeField] private EnemyWeapon rangedWeapon;
        [SerializeField] private EnemyWeapon particleWeapon;
        #endregion

        #region Dependencies
        #endregion

        #region Properties
        public IEnemyModel EnemyModel { get; private set; }
        public Vector3 Position => transform.position;
        public Transform HealthBarAnchor => healthBarAnchor;
        public Rigidbody Rigidbody => rigidBody;
        #endregion

        #region State
        private int meleeDamage;
        private int particleDamage;
        private int rangedDamage;
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

        public virtual void MeleeAttack(string attackAnim)
        {
            if (meleeWeapon == null)
            {
                throw new System.Exception($"[Enemy] {EnemyModel.Settings.Id} does not have a melee weapon");
            }
            animator.SetTrigger(attackAnim);
            ChangeEnemyState(EnemyState.MeleeAttack);
            meleeWeapon.Attack(() => ChangeEnemyState(EnemyState.Moving));
        }
        public virtual void ParticleAttack(string attackAnim)
        {
            if (particleWeapon == null)
            {
                throw new System.Exception($"[Enemy] {EnemyModel.Settings.Id} does not have a particle weapon");
            }
            animator.SetTrigger(attackAnim);
            ChangeEnemyState(EnemyState.ParticleAttack);
            particleWeapon.Attack(() => ChangeEnemyState(EnemyState.Moving));
        }
        public virtual void RangedAttack(string attackAnim)
        {
            if (rangedWeapon == null)
            {
                throw new System.Exception($"[Enemy] {EnemyModel.Settings.Id} does not have a ranged weapon");
            }
            animator.SetTrigger(attackAnim);
            ChangeEnemyState(EnemyState.RangedAttack);
            rangedWeapon.Attack(() => ChangeEnemyState(EnemyState.Moving));
        }

        public void ChangeEnemyState(EnemyState state)
        {
            EnemyModel.EnemyState = state;
        }
        #endregion

        #region Private
        private void OnDamageTaken(int damage)
        {
            animator.SetTrigger("Take Damage");
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
