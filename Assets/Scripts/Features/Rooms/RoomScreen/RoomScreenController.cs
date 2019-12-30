using Core.CameraScripts;
using Core.IoC;
using Core.ResourceManagement;
using Features.Enemies;
using Features.Heroes;
using Features.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Features.Rooms.Screens
{
    public class RoomScreenController: DualScreenController<RoomScreen2D, RoomScreen3D>
    {
        #region - Constants
        public const string ScreenName = "RoomScreen";  
        private const string EnemyPath = "EnemyPrefabs/";
        #endregion

        #region - Properties
        public override string Name => ScreenName;
        public TopDownCamera TopDownCamera { get; private set; }
        #endregion

        #region - Dependencies
        [Inject] private IHeroModel heroModel;
        [Inject] private IResourceManager resourceManager;
        [Inject] private IEnemiesModel enemiesModel;
        #endregion

        #region - State
        private Hero hero;
        private Dictionary<string, Enemy> enemyPrefabCache = new Dictionary<string, Enemy>();
        private Dictionary<int, Enemy> enemiesOnScreen = new Dictionary<int, Enemy>();
        #endregion

        #region - Lifecycle
        public override void Init()
        {
            TopDownCamera = Camera.main.GetComponent<TopDownCamera>();
            if (TopDownCamera == null)
            {
                throw new Exception("[RoomScreenController] No TopDownCamera component found on main camera.");
            }

            hero = heroModel.CreateHero(Screen3D.HeroContainer);
            hero.OnHitEnemy += HandleHitEnemy;
            TopDownCamera.CameraTarget = hero.transform;
            Screen2D.InstantiateHeroHealthBar(hero.HealthBarAnchor, Camera.main, heroModel);
            enemiesModel.Init();
            enemiesModel.OnDeath += HandleEnemyDeath;
            enemiesModel.OnPlayerHit += HandlePlayerHit;
            SpawnEnemies();
            Screen2D.Joystick.OnUpdate += HandlePlayerInput;
        }

        #endregion

        #region - Private
        private void SpawnEnemies()
        {
            foreach (var enemy in enemiesModel)
            {
                var settings = enemy.Settings;
                var split = settings.Id.Split('_');
                var path = EnemyPath + split[0] + "/" + settings.Id;
                var enemyTemplate = enemyPrefabCache.TryGetValue(settings.Id, out var template)
                    ? template
                    : resourceManager.LoadResource<Enemy>(path);
                enemyPrefabCache[settings.Id] = enemyTemplate;
                var enemyInstance = UnityEngine.Object.Instantiate(enemyTemplate, Screen3D.EnemyContainer);
                enemyInstance.Init(enemy);
                Screen2D.InstantiateEnemyHealthBar(enemyInstance.HealthBarAnchor, Camera.main, enemy);
                enemiesOnScreen.Add(enemy.Index, enemyInstance);
            }
        }
        private void HandlePlayerInput(bool isPointerDown)
        {
            if (heroModel.CurrentState == HeroState.Dead)
            {
                return;
            }

            if (isPointerDown)
            {
                var input = new Vector3(Screen2D.Joystick.Horizontal, 0, Screen2D.Joystick.Vertical);
                hero.MoveCharacter(input);
            }
            else
            {
                hero.Shoot(FindClosestEnemy());
            }
        }

        private Vector3 FindClosestEnemy()
        {
            var orderedByDistance = enemiesOnScreen.OrderBy(x => Vector3.Distance(hero.Position, x.Value.Position));
            return orderedByDistance.First().Value.Position;
        }

        private void HandleHitEnemy(int enemyIndex)
        {
            enemiesModel.ApplyDamage(enemyIndex, heroModel.GetCurrentHeroAttack); 
        }

        private void HandleEnemyDeath(IEnemyModel model)
        {
            var index = model.Index;
           
            if (!enemiesOnScreen.TryGetValue(index, out var enemy))
            {
                throw new Exception("[RoomScreenController] No enemy on screen with Id " + index);
            }

            enemy.PlayDeathAnimation();
            enemiesOnScreen.Remove(index);

            if (enemiesOnScreen.Count == 0)
            {
                enemiesModel.GenerateNextWave();
                SpawnEnemies();
            }
        }

        private void HandlePlayerHit(int damage)
        {
            heroModel.ApplyDamage(damage);
        }
        #endregion
    }
}
