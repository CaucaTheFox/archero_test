using Features.Enemies;
using Features.Heroes;
using UI;
using UnityEngine;

namespace Features.Rooms.Screens
{
    public class RoomScreen2D : MonoBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private Joystick joystick;
        [SerializeField] private EnemyHealthBarView enemyHealthBarTemplate;
        [SerializeField] private HeroHealthBarView heroHealthBarTemplate;
        [SerializeField] private Transform markerViewCollection;
        #endregion

        #region Properties
        public Joystick Joystick => joystick;
        #endregion

        #region Public
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
        #endregion
    }
}