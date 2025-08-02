using Core.CameraScripts;
using Core.IoC;
using Core.ResourceManagement;
using Features.Enemies;
using Features.Heroes;
using Features.Screens;
using System.Linq;
using UnityEngine;
using Utility.Utility;

namespace Features.Rooms.Screens
{
    public class RoomScreenController: DualScreenController<RoomScreen2D, RoomScreen3D>
    {
        #region Constants
        public const string ScreenName = "RoomScreen";  
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
        private int waveCount;
        #endregion

        #region Lifecycle
        public override void Init()
        {
            SpawnHero();
            Screen2D.InstantiateHeroHealthBar(hero.HealthBarAnchor, Camera.main, heroModel);
            
            TopDownCamera = Camera.main.GetComponent<TopDownCamera>();
            TopDownCamera.CameraTarget = hero.transform;
            
            enemiesModel.OnDeath += HandleEnemyDeath;
            enemiesModel.OnPlayerHit += HandlePlayerHit;
            SpawnEnemyHealthBars();
            
            Screen2D.Joystick.OnFixedUpdate += HandlePlayerInput;
            Screen3D.OnPlayerHit += HandlePlayerHit;
            WaveCount = 1;
        }

        #endregion

        #region Private

        private void SpawnHero()
        {
            hero = heroModel.CreateHero(Screen3D.HeroContainer);
            hero.OnHitEnemy += HandleEnemyHit;
            heroModel.OnDeath += HandlePlayerDeath;
        }
        
        private void SpawnEnemyHealthBars()
        {
            foreach (var entry in enemiesModel.EnemyModels)
            {
                var enemyInstance = entry.Value.EnemyInstance;
                enemyInstance.transform.SetParent(Screen3D.EnemyContainer);
                Screen2D.SpawnEnemyHealthBar(enemyInstance.HealthBarAnchor, Camera.main, entry.Value);
            }
        }
        
        private void HandlePlayerInput(bool isPointerDown)
        {
            if (heroModel.CurrentState == HeroState.Dead)
                return;

            if (enemiesModel.EnemyModels.Count == 0)
                return;
            
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
            var orderedByDistance = enemiesModel.EnemyModels
                .Select(x => x.Value.EnemyInstance)
                .OrderBy(x => Vector3.Distance(hero.Position, x.Position));
                
            return orderedByDistance.First();
        }

        private void HandleEnemyHit(int enemyIndex)
        {
            enemiesModel.ApplyDamage(enemyIndex, heroModel.GetCurrentHeroAttack); 
        }

        private void HandleEnemyDeath()
        {
            if (enemiesModel.EnemyModels.Count != 0) 
                return;
            
            enemiesModel.SpawnEnemyWave();
            SpawnEnemyHealthBars();
            WaveCount += 1;
        }

        private void HandlePlayerHit(int damage)
        {
            heroModel.ApplyDamage(damage);
        }

        private void HandlePlayerDeath()
        {
            enemiesModel.Cleanup();
            enemiesModel.OnPlayerHit -= HandlePlayerHit;
            enemiesModel.OnDeath -= HandleEnemyDeath;
            
            Screen3D.OnPlayerHit -= HandlePlayerHit;
            
            Screen2D.ShowGameOverPanel(waveCount);
            Screen2D.OnReset += HandleGameReset;
        }

        private void HandleGameReset()
        {
            Screen2D.HideGameOverPanel();
            Screen2D.OnReset -= HandleGameReset;
            Screen2D.Joystick.OnFixedUpdate -= HandlePlayerInput;
            
            hero.OnHitEnemy -= HandleEnemyHit;
            heroModel.OnDeath -= HandlePlayerDeath;
            Screen3D.HeroContainer.DestroyChildren();
            hero = null;
            Init();
        }
        #endregion
    }
}
