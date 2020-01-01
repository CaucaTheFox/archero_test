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
        private const string RunAnim = "Run";
        private const string TakeDamageAnim = "Take Damage";
        private const string DeathAnim = "Die";
        private const string DodgeAnim = "Defend";
        private const string ShootAnim = "Arrow Attack";
        #endregion

        #region Unity Serialized Fields
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform healthBarAnchor;
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
            transform.Translate(joyStickInput * navMeshAgent.speed * Time.deltaTime, Space.World);
            if (joyStickInput != Vector3.zero)
            {
                transform.forward = joyStickInput;
            }
            animator.SetTrigger(RunAnim);
        }

        public void Shoot(Vector3 closestEnemyPosition)
        {
            isMoving = false;
            animator.SetTrigger(ShootAnim);
            animator.speed = Settings.AttackSpeed;
            transform.LookAt(closestEnemyPosition);
        }

        public void PlayDamageAnimation()
        {
            animator.SetTrigger(TakeDamageAnim);
        }

        public void PlayDeathAnimation()
        {
            animator.SetTrigger(DeathAnim);
        }

        public void PlayDodgeAnim()
        {
            animator.SetTrigger(DodgeAnim);
        }
        #endregion

        #region Private

        private void ShowArrow()
        {
            if (isMoving)
            {
                return;
            }

            var arrow = GameObject.Instantiate<Arrow>(arrowPrefab);
            arrow.transform.position = Position;
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
