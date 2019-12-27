using Features.Heroes;

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
            base.SetData(heroModel.CurrentHealthNormalized, heroModel.CurrentHealth);
        }
        #endregion

        #region Private
        private void UpdateData()
        {
            base.TweenedUpdate(heroModel.CurrentHealthNormalized, heroModel.CurrentHealth);
        }
        #endregion
    }
}
