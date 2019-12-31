using Features.Heroes;
using UnityEngine;

namespace UI
{
    public class HeroHealthBarView : HealthBarView
    {
        #region State
        private IHeroModel heroModel;
        #endregion

        #region Public
        public void SetData(IHeroModel model)
        {
            heroModel = model;
            heroModel.OnDamageTaken += HandleDamageTaken;
            heroModel.OnDeath += HandleDeath;
            base.SetData(heroModel.CurrentHealthNormalized, heroModel.CurrentHealth);
        }
        #endregion

        #region Private
        private void HandleDamageTaken()
        {
            base.TweenedUpdate(heroModel.CurrentHealthNormalized, heroModel.CurrentHealth);
        }
        private void HandleDeath()
        {
            base.TweenedUpdate(heroModel.CurrentHealthNormalized, heroModel.CurrentHealth);
            GameObject.Destroy(gameObject, 0.5f);
        }

        private void OnDestroy()
        {
            heroModel.OnDamageTaken -= HandleDamageTaken;
            heroModel.OnDeath -= HandleDeath;
        }
        #endregion
    }
}
