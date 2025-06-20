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
using Utility.Utility;
using Object = UnityEngine.Object;

namespace Features.Rooms.Screens
{
    public class RoomScreenController: DualScreenController<RoomScreen2D, RoomScreen3D>
    {
        #region Constants
        public const string ScreenName = "RoomScreen";  
        private const string EnemyPath = "EnemyPrefabs/";
        #endregion

        #region Dependencies
        [Inject] private IHeroModel heroModel;
        [Inject] private IResourceManager resourceManager;
        [Inject] private IEnemiesModel enemiesModel;
        #endregion
        
        #region Properties
        public override string Name => ScreenName;
        private TopDownCamera TopDownCamera { get; set; }

        private int WaveCount
        {
            get => waveCount;
            set
            {
                waveCount = value;
                Screen2D.UpdateWaveCount(value);
            }
        }
        #endregion
        
        #region State
        private Hero hero;
        private Dictionary<string, Enemy> enemyPrefabCache = new Dictionary<string, Enemy>();
        private Dictionary<int, Enemy> enemiesOnScreen = new Dictionary<int, Enemy>();
        private int waveCount;
        #endregion

        #region Lifecycle
        public override void Init()
        {
            TopDownCamera = Camera.main.GetComponent<TopDownCamera>();

            hero = heroModel.CreateHero(Screen3D.HeroContainer);
            hero.OnHitEnemy += HandleHitEnemy;
            heroModel.OnDeath += HandlePlayerDeath;
            TopDownCamera.CameraTarget = hero.transform;
            Screen2D.InstantiateHeroHealthBar(hero.HealthBarAnchor, Camera.main, heroModel);
            enemiesModel.Init();
            enemiesModel.OnDeath += HandleEnemyDeath;
            enemiesModel.OnPlayerHit += HandlePlayerHit;
            SpawnEnemies();
            Screen2D.Joystick.OnFixedUpdate += HandlePlayerInput;
            Screen3D.OnPlayerHit += HandlePlayerHit;
            WaveCount = 1;
        }

        #endregion

        #region Private
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
                var closestEnemy = FindClosestEnemy();
                hero.Shoot(closestEnemy.Position);
            }
        }

        private Enemy FindClosestEnemy()
        {
            var orderedByDistance = enemiesOnScreen
                .OrderBy(x => Vector3.Distance(hero.Position, x.Value.Position));
                
            return orderedByDistance.First().Value;
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

            if (enemiesOnScreen.Count != 0) 
                return;
            
            enemiesModel.GenerateNextWave();
            SpawnEnemies();
            WaveCount += 1;
        }

        private void HandlePlayerHit(int damage)
        {
            heroModel.ApplyDamage(damage);
        }

        private void HandlePlayerDeath()
        {
            foreach (var enemy in enemiesOnScreen)
            {
                Object.Destroy(enemy.Value.gameObject);
            }

            enemiesOnScreen.Clear();
            enemiesModel.OnPlayerHit -= HandlePlayerHit;
            enemiesModel.OnDeath -= HandleEnemyDeath;
            Screen3D.OnPlayerHit -= HandlePlayerHit;

            Screen2D.ShowGameOverPanel(waveCount);
            Screen2D.OnReset += HandleGameReset;
        }

        private void HandleGameReset()
        {
            Screen2D.OnReset -= HandleGameReset;
            Screen2D.HideGameOverPanel();

            Screen2D.Joystick.OnFixedUpdate -= HandlePlayerInput;
            hero.OnHitEnemy -= HandleHitEnemy;
            heroModel.OnDeath -= HandlePlayerDeath;
            Screen3D.HeroContainer.DestroyChildren();
            hero = null;
            Init();
        }
        #endregion
    }
}
