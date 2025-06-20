using Features.Enemies;

namespace UI
{
    public class EnemyHealthBarView : HealthBarView
    {
        #region State
        private IEnemyModel enemyModel;
        #endregion

        public void SetData(IEnemyModel model)
        {
            enemyModel = model;
            enemyModel.OnDamageTaken += HandleDamageTaken;
            enemyModel.OnDeath += HandleDeath;
            enemyModel.OnVisibilityChange += HandleVisibilityChange;
            base.SetData(enemyModel.CurrentHealthNormalized, 0);
        }

        #region Private
        private void HandleDamageTaken(int _)
        {
            TweenedUpdate(enemyModel.CurrentHealthNormalized, 0);
        }
        private void HandleDeath(int _)
        {
            TweenedUpdate(enemyModel.CurrentHealthNormalized, 0);
            DestroyBar();
        }

        private void HandleVisibilityChange()
        {
            gameObject.SetActive(enemyModel.IsVisible);
        }

        private void DestroyBar()
        {
            Destroy(gameObject, 1f);
        }

        private void OnDestroy()
        {
            enemyModel.OnDamageTaken -= HandleDamageTaken;
            enemyModel.OnDeath -= HandleDeath;
            enemyModel.OnVisibilityChange -= HandleVisibilityChange;
        }
        #endregion
    }
}
