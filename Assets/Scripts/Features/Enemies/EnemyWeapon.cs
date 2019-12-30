using Features.Heroes;
using System;
using System.Collections;
using UnityEngine;

namespace Features.Enemies
{
    public class EnemyWeapon : MonoBehaviour
    {
        private enum WeaponType
        {
            Melee, 
            Particle, 
            Ranged,
        }

        #region Events
        public event Action OnPlayerHit;
        #endregion

        #region Unity Serialized Fields
        [SerializeField] private float visibilityDuration;
        [SerializeField] private WeaponType weaponType;
        #endregion

        #region Lifecycle
        #endregion

        #region Public
        protected virtual void OnTriggerEnter(Collider collider)
        {
            var hitHero = collider.GetComponentInParent<Hero>();
            if (hitHero != null)
            {
                OnPlayerHit?.Invoke();
            }
        }

        public void Attack(Action callBack)
        {
            switch(weaponType)
            {
                case WeaponType.Particle:
                    ParticleAttack(callBack);
                    break;
                case WeaponType.Ranged:
                    RangedAttack(callBack);
                    break;
            }
        }
        #endregion

        #region Private
        private void ParticleAttack(Action callBack)
        {
            gameObject.SetActive(true);
            StartCoroutine(DelayedAttackEnd());

            IEnumerator DelayedAttackEnd()
            {
                yield return new WaitForSeconds(visibilityDuration);
                gameObject.SetActive(false);
                callBack?.Invoke();
            }
        }

        private void RangedAttack(Action callBack)
        {
        }
        #endregion
    }
}
