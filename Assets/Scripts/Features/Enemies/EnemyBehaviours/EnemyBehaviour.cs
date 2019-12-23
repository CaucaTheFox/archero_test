using Core.IoC;
using Features.Heroes;
using UnityEngine;

namespace Features.Enemies
{
    public class EnemyBehaviour : InjectableBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private Enemy enemy;
        #endregion

        #region Dependencies
        [Inject] private IHeroModel heroModel;
        #endregion

        #region Properties
        #endregion

        #region Lifecycle
        public virtual void Update()
        {
            MoveTowardsHero();
        }
        #endregion

        #region Public
        public virtual void Init()
        {
          
        }

        public virtual void MoveTowardsHero()
        {
            enemy.MoveTorwardsTarget(heroModel.HeroPosition);
        }
        #endregion
    }
}
