using Core.IoC;
using System;
using UnityEngine;
using UnityEngine.AI;
using Utility;

namespace Features.Heroes
{
    public class Hero : InjectableBehaviour
    {
        #region Events
        public event Action<int> OnHitEnemy;
        #endregion

        #region Constants
        private static readonly int RunTrigger = Animator.StringToHash("Run");
        private static readonly int ShootTrigger = Animator.StringToHash("Arrow Attack");
        private static readonly int TakeDamageTrigger = Animator.StringToHash("Take Damage");
        private static readonly int DeathTrigger = Animator.StringToHash("Die");
        #endregion

        #region Unity Serialized Fields
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform healthBarAnchor, arrowAnchor;
        [SerializeField] private Arrow arrowPrefab;
        [SerializeField] private AnimationEventProvider animationEventProvider;
        #endregion

        #region Properties
        public HeroSettings Settings {get;set;}
        public Vector3 Position => transform.position;
        public Transform HealthBarAnchor => healthBarAnchor;
        #endregion

        #region State
        private bool isMoving;
        #endregion

        #region Lifecycle
        private void Start()
        {
            animationEventProvider.OnAttack += ShowArrow;
            animator.speed = 1;
        }
        private void OnDestroy()
        {
            animationEventProvider.OnAttack -= ShowArrow;
        }
        #endregion

        #region Public
        public void MoveCharacter(Vector3 joyStickInput)
        {
            isMoving = true;
            animator.speed = 1;
            animator.ResetTrigger(ShootTrigger);
            animator.SetTrigger(RunTrigger);
            transform.Translate(joyStickInput * (navMeshAgent.speed * Time.deltaTime), Space.World);
            if (joyStickInput != Vector3.zero)
            {
                transform.forward = joyStickInput;
            }
        }

        public void Shoot(Vector3 closestEnemyPosition)
        {
            isMoving = false; 
            animator.speed = Settings.AttackSpeed;
            animator.ResetTrigger(RunTrigger);
            animator.SetTrigger(ShootTrigger);
            transform.LookAt(closestEnemyPosition);
        }

        public void PlayDamageAnimation()
        {
            animator.SetTrigger(TakeDamageTrigger);
        }

        public void PlayDeathAnimation()
        {
            animator.SetTrigger(DeathTrigger);
        }
        #endregion

        #region Private

        private void ShowArrow()
        {
            if (isMoving)
                return;

            var arrow = Instantiate(arrowPrefab);
            arrow.transform.position = arrowAnchor.position;
            arrow.transform.up = -transform.forward;
            arrow.FlightDirection = transform.forward;
            arrow.OnHitEnemy += DispatchHitEnemy;
            
            void DispatchHitEnemy(int enemyIndex)
            {
                arrow.OnHitEnemy -= DispatchHitEnemy;
                OnHitEnemy?.Invoke(enemyIndex);
            }
        }
        #endregion
    }
}
