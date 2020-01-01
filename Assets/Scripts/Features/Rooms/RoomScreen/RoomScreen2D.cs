using Features.Enemies;
using Features.Heroes;
using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utility.Utility;

namespace Features.Rooms.Screens
{
    public class RoomScreen2D : MonoBehaviour
    {
        #region Events
        public event Action OnReset;
        #endregion

        #region Unity Serialized Fields
        [SerializeField] private Joystick joystick;
        [SerializeField] private EnemyHealthBarView enemyHealthBarTemplate;
        [SerializeField] private HeroHealthBarView heroHealthBarTemplate;
        [SerializeField] private Transform markerViewCollection, gameOverPanel;
        [SerializeField] private Button resetButton;
        [SerializeField] private TextMeshProUGUI waveCountLabel, gameOverLabel;
        #endregion

        #region Properties
        public Joystick Joystick => joystick;
        #endregion

        #region Public
        public void UpdateWaveCount(int waveCount)
        {
            waveCountLabel.text = "Wave " + waveCount;
        }
        public void InstantiateHeroHealthBar(Transform target, Camera camera, IHeroModel heroModel)
        {
            var healthBar = Instantiate(heroHealthBarTemplate, markerViewCollection);
            healthBar.SetData(heroModel);
            healthBar.SetTarget(target, camera);
        }

        public void InstantiateEnemyHealthBar(Transform target, Camera camera, IEnemyModel enemyModel)
        {
            var healthBar = Instantiate(enemyHealthBarTemplate, markerViewCollection);
            healthBar.SetData(enemyModel);
            healthBar.SetTarget(target, camera);
        }

        public void ShowGameOverPanel(int waveCount)
        {
            gameOverPanel.gameObject.SetActive(true);
            gameOverLabel.text = $"You fell in wave {waveCount}. Can you do better?";
            markerViewCollection.DestroyChildren();
            resetButton.onClick.AddListener(() => OnReset?.Invoke());
        }

        public void HideGameOverPanel()
        {
            gameOverPanel.gameObject.SetActive(false);
            resetButton.onClick.RemoveAllListeners();
        }
        #endregion
    }
}