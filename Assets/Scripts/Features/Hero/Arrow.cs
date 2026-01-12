using Features.Enemies;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Heroes
{
    public class Arrow : MonoBehaviour
    {
        #region Events
        public event Action<int> OnHitEnemy;
        #endregion

        #region Unity Serialized Fields
        [SerializeField] private float arrowSpeed, arrowLifeTime;
        #endregion

        #region Properties
        public Vector3 Position => transform.position;
        public Vector3 FlightDirection { get; set; }
        #endregion

        #region State
        private float lifeTime;
        private bool hasHit;
        private Coroutine flightRoutine;
        private WaitForEndOfFrame waitForEndOfFrame;
        #endregion

        #region Lifecycle

        public void Init(Vector3 position, Vector3 direction)
        {
            lifeTime = 0;
            transform.position = position;
            transform.up = -direction;
            FlightDirection = direction;
            waitForEndOfFrame = new WaitForEndOfFrame(); 
            flightRoutine = StartCoroutine(FlightRoutine());
        }
        
        private IEnumerator FlightRoutine()
        {
            if (hasHit)
                yield break;
            
            while (lifeTime < arrowLifeTime)
            {
                lifeTime += Time.deltaTime;
                var movement = FlightDirection * (Time.deltaTime * arrowSpeed) + transform.position;
                movement.y = 0.5f;
                transform.position = movement;
                yield return waitForEndOfFrame;
            }
          
            if (lifeTime >= arrowLifeTime)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Public
        public void OnTriggerEnter(Collider collider)
        {
            var obstacle = collider.gameObject.GetComponent<NavMeshObstacle>();
            if (obstacle != null)
            {
                hasHit = true;
                Destroy(gameObject, 0.5f);
                return;
            }

            var hitEnemy = collider.GetComponentInParent<Enemy>();
            if (hitEnemy == null) 
                return;
            
            OnHitEnemy?.Invoke(hitEnemy.InstanceId);
            hasHit = true;
            Destroy(gameObject);
        }
        #endregion
    }
}
