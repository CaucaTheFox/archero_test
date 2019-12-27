using Core.IoC;
using Features.Heroes;
using UnityEngine;

namespace Features.Enemies
{
    public class EnemyBehaviour : InjectableBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] protected Enemy enemy;
        #endregion

        #region Dependencies
        [Inject] public IHeroModel heroModel;
        #endregion

        #region Lifecycle
        public virtual void Update()
        {
            MoveTowardsHero();  
        }
        #endregion

        #region Public
        #endregion

        #region Private
        private void MoveTowardsHero()
        {
            enemy.MoveTorwardsTarget(heroModel.HeroPosition);
        }
        #endregion
    }
}
