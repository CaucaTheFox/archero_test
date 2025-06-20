using Features.Heroes;
using System;
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
            Collision
        }

        #region Events
        public event Action OnPlayerHit;
        #endregion

        #region Unity Serialized Fields
        [SerializeField] private WeaponType weaponType;
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

        public void Attack()
        {
            switch(weaponType)
            {
                case WeaponType.Particle:
                    ShowParticle();
                    break;
                case WeaponType.Ranged:
                    RangedAttack();
                    break;
            }
        }

        public void HideParticle()
        {
            gameObject.SetActive(false);
        }
        #endregion

        #region Private
        private void ShowParticle()
        {
            gameObject.SetActive(true);
        }

        private void RangedAttack()
        {
        }
        #endregion
    }
}
