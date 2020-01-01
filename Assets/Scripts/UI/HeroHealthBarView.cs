using Features.Heroes;
using UnityEngine;
using DG.Tweening;

namespace UI
{
    public class HeroHealthBarView : HealthBarView
    {
        #region Unity Serialized Fields
        [SerializeField] private Transform attackDodgedPanel;
        #endregion

        #region State
        private IHeroModel heroModel;
        #endregion

        #region Public
        public void SetData(IHeroModel model)
        {
            heroModel = model;
            heroModel.OnDamageTaken += HandleDamageTaken;
            heroModel.OnDeath += HandleDeath;
            heroModel.OnDodge += HandleDodge;
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

        private void HandleDodge()
        {
            attackDodgedPanel.transform
                .DOScale(Vector3.one * 1.5f, 0.5f)
                .SetEase(Ease.OutBounce)
                .OnComplete(() =>
                    attackDodgedPanel.transform
                        .DOScale(Vector3.zero, 0.5f)
                        .SetEase(Ease.InBounce)
                    );
        }


        private void OnDestroy()
        {
            heroModel.OnDamageTaken -= HandleDamageTaken;
            heroModel.OnDeath -= HandleDeath;
            heroModel.OnDodge -= HandleDodge;
        }
        #endregion
    }
}
