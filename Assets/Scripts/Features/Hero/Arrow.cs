using Features.Enemies;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Heroes
{
    public class Arrow : MonoBehaviour
    {
        #region Events
        public event Action<Enemy> OnHitEnemy;
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
        private bool hitObstacle;
        #endregion

        #region Lifecycle
        private void Start()
        {
            lifeTime = 0;
        }
        private void Update()
        {
            if (hitObstacle)
            {
                return;
            }

            lifeTime += Time.deltaTime;
            if (lifeTime >= arrowLifeTime)
            {
                GameObject.Destroy(gameObject);
                return;
            }

            var movement = FlightDirection * Time.deltaTime * arrowSpeed;
            movement.y = 0;
            transform.position += movement;
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
                hitObstacle = true;
                GameObject.Destroy(gameObject, 0.5f);
                return;
            }

            var hitEnemy = collider.gameObject.GetComponent<Enemy>();
            if (hitEnemy != null)
            {
                OnHitEnemy?.Invoke(hitEnemy);
                GameObject.Destroy(gameObject);
            }
        }
        #endregion

        #region Private
        #endregion
    }
}
