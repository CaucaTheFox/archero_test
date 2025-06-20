using Features.Enemies;
using System;
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
        #endregion

        #region Lifecycle
        private void Start()
        {
            lifeTime = 0;
        }
        private void Update()
        {
            if (hasHit)
            {
                return;
            }

            lifeTime += Time.deltaTime;
            if (lifeTime >= arrowLifeTime)
            {
                GameObject.Destroy(gameObject);
                return;
            }

            var movement = FlightDirection * (Time.deltaTime * arrowSpeed) + transform.position;
            movement.y = 0.5f;
            transform.position = movement;
        }
        private void OnDestroy()
        {

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
            if (hitEnemy != null)
            {
                OnHitEnemy?.Invoke(hitEnemy.EnemyModel.Index);
                hasHit = true;
                Destroy(gameObject);
            }
        }
        #endregion
    }
}
