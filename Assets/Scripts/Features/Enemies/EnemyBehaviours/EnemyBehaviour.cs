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
        protected virtual void Update()
        {
            Move();
        }
        #endregion

        #region Public     
        protected virtual void Move()
        {
            enemy.MoveTorwardsTarget(heroModel.HeroPosition);
        }

        protected virtual void Attack()
        {
        
        }
        #endregion

    }
}
