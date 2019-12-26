using Core.IoC;
using Core.Time;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Heroes
{
    public class Hero : InjectableBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private Arrow arrowPrefab;
        #endregion

        #region Dependencies
        #endregion

        #region Properties
        public HeroSettings Settings {get;set;}
        public Vector3 Position => transform.position;
        #endregion

        #region State
        private Coroutine arrowInstantiationRoutine;
        #endregion

        #region Lifecycle
        private void Start()
        {

        }
        private void OnDestroy()
        {
            StopArrowShooting();
        }
        #endregion

        #region Public
        public void MoveCharacter(Vector3 joyStickInput)
        {
            StopArrowShooting();
            
            transform.Translate(joyStickInput * navMeshAgent.speed * Time.deltaTime, Space.World);
            if (joyStickInput != Vector3.zero)
            {
                transform.forward = joyStickInput;
            }
            animator.SetTrigger("Run");
        }

        public void Shoot()
        {
            animator.SetTrigger("Arrow Attack");
            if (arrowInstantiationRoutine == null)
            {
                arrowInstantiationRoutine = StartCoroutine(InstantiateArrows()); 
            }
        }
        #endregion

        #region Private

        private void StopArrowShooting()
        {
            if (arrowInstantiationRoutine != null)
            {
                StopCoroutine(arrowInstantiationRoutine);
                arrowInstantiationRoutine = null;
            }
        }
        private IEnumerator InstantiateArrows()
        {
            while(true)
            {
                yield return new WaitForSeconds(1 / Settings.AttackSpeed);
                var arrow = GameObject.Instantiate<Arrow>(arrowPrefab);
                arrow.transform.position = Position;
                arrow.transform.up = -transform.forward;
                arrow.FlightDirection = transform.forward; 
            }
        }

        #endregion
    }
}
