using Features.Heroes;
using System;
using UnityEngine;

namespace Features.Rooms
{
    public class TrapTile : Tile
    {
        #region Events
        public event Action<int> OnPlayerHit;
        #endregion

        #region Unity Serialized Fields
        [SerializeField] private int damage;
        [SerializeField] private ParticleSystem impactParticles;
        #endregion

        #region Public
        public void OnTriggerEnter(Collider collider)
        {
            var hitHero = collider.GetComponentInParent<Hero>();
            if (hitHero != null)
            {
                impactParticles.Play();
                OnPlayerHit?.Invoke(damage);
            }
        }
        #endregion
    }
}
