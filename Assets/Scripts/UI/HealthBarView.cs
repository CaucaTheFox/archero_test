using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBarView : MarkerView
    {
        #region Unity Serialized Fields
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI totalHealthAmount;
        #endregion

        #region Protected

        protected void SetData(float percentage, int totalHealth)
        {
            fillImage.fillAmount = percentage;
            if (totalHealthAmount != null)
            {
                totalHealthAmount.text = totalHealth.ToString();
            }
        }

        protected void TweenedUpdate(float percentage, int health)
        {
            fillImage.DOFillAmount(percentage, 0.5f).SetEase(Ease.InQuint);
            if (totalHealthAmount != null)
            {
                totalHealthAmount.text = health.ToString();
            }
        }
        #endregion
    }
}