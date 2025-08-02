using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Core.CoroutineHelper;
using Core.IoC;
using Core.ResourceManagement;
using UnityEngine;

namespace Features.Enemies
{
    public interface IEnemyModel
    {
        event Action<int> OnDamageTaken;
        event Action<int> OnDeath;
        event Action<int> OnPlayerHit;
        event Action OnVisibilityChange;

        int InstanceId { get; }
        float CurrentHealthNormalized { get; }
        bool IsDead { get; }
        bool IsVisible { get; set; }
        EnemySettings Settings { get; }
        EnemyState EnemyState { get; set; }
        Enemy EnemyInstance { get;}
        
        void ApplyDamage(int damage);
    }
    
    public enum EnemyState
    {
        Alive,
        Dead,
    }

    public class EnemyModel : IEnemyModel
    {
        #region Events
        public event Action<int> OnDamageTaken;
        public event Action<int> OnDeath;
        public event Action<int> OnPlayerHit;
        public event Action OnVisibilityChange;
        #endregion

        #region Constants
        private const string EnemyPath = "EnemyPrefabs/";
        #endregion
        
        #region Dependencies
        [Inject] private IResourceManager resourceManager;
        [Inject] private ICoroutineHelper coroutineHelper;
        
        [Inject] private IJsonConfig<EnemyBehaviourCatalogueConfig> enemyBehaviourCatalogueConfig;
        #endregion
        
        #region Properties
        public int InstanceId { get; }
        public float CurrentHealthNormalized => (float) currentHealth / Settings.Health;
        public bool IsDead => EnemyState == EnemyState.Dead;
        public bool IsVisible {
            get => isVisible;
            set
            {
                isVisible = value;
                OnVisibilityChange?.Invoke();
            }
        }

        public EnemySettings Settings { get; }
        public EnemyState EnemyState { get; set; }
        public Enemy EnemyInstance { get; private set; }
        #endregion

        #region State     
        private bool isVisible;
        private int currentHealth;
        private Coroutine enemyBehaviourRoutine;
        private Dictionary<EnemyBehaviourActionType, EnemyBehaviourAction> behaviourActions;
        #endregion

        #region Lifecycle
        public EnemyModel(int instanceId, EnemySettings settings)
        {
            GameContext.Container.ResolveAll(this);
            
            InstanceId = instanceId;
            Settings = settings;
            currentHealth = settings.Health;
            EnemyState = EnemyState.Alive;
            isVisible = true;
            
            SpawnEnemyInstance();
            StartEnemyBehaviourRoutine();
        }
        
        public void Cleanup()
        {
            StopEnemyBehaviourRoutine();
            EnemyInstance.OnPlayerHit -= DispatchPlayerHit;
            UnityEngine.Object.Destroy(EnemyInstance);
            EnemyInstance = null;
        }
        #endregion
        
        #region Public
        public void ApplyDamage(int damage)
        {
            if (EnemyState == EnemyState.Dead || !IsVisible)
                return;

            currentHealth -= damage;
            if (currentHealth > 0)
            {
                OnDamageTaken?.Invoke(InstanceId);
                EnemyInstance.PlayDamageAnimation();
            }
            else
            {
                EnemyState = EnemyState.Dead;
                StopEnemyBehaviourRoutine();
                EnemyInstance.PlayDeathAnimation();
                OnDeath?.Invoke(InstanceId);
            }
        }
        #endregion

        #region Private
        private void SpawnEnemyInstance()
        {
            var split = Settings.Id.Split('_');
            var path = EnemyPath + Settings.Id;
            var enemyTemplate = resourceManager.LoadResource<Enemy>(path);
            EnemyInstance = UnityEngine.Object.Instantiate(enemyTemplate);
            EnemyInstance.OnPlayerHit += DispatchPlayerHit;
        }

        private void StartEnemyBehaviourRoutine()
        {
            behaviourActions = new Dictionary<EnemyBehaviourActionType, EnemyBehaviourAction>();
            var enemyBehaviourConfig = enemyBehaviourCatalogueConfig.Value.Configs[Settings.EnemyBehaviourConfigId];
            foreach (var actionData in enemyBehaviourConfig.EnemyBehaviourActionData)
            {
                switch (actionData.GetEnemyBehaviourActionType())
                {
                    case EnemyBehaviourActionType.Idle:
                        var idleEnemyBehaviourAction = new IdleEnemyBehaviourAction();
                        idleEnemyBehaviourAction.Init(this);
                        behaviourActions.TryAdd(EnemyBehaviourActionType.Idle, idleEnemyBehaviourAction);
                        break;
                    case EnemyBehaviourActionType.Movement:
                        var movementEnemyBehaviourAction = new MovementEnemyBehaviourAction();
                        movementEnemyBehaviourAction.Init(this);
                        behaviourActions.TryAdd(EnemyBehaviourActionType.Movement, movementEnemyBehaviourAction);
                        break;
                    case EnemyBehaviourActionType.Dash:
                        var dashEnemyBehaviourAction = new DashEnemyBehaviourAction();
                        dashEnemyBehaviourAction.Init(this);
                        behaviourActions.TryAdd(EnemyBehaviourActionType.Dash, dashEnemyBehaviourAction);
                        break;
                    case EnemyBehaviourActionType.Attack:
                        var attackEnemyBehaviourAction = new AttackEnemyBehaviourAction();
                        attackEnemyBehaviourAction.Init(this);
                        behaviourActions.TryAdd(EnemyBehaviourActionType.Attack, attackEnemyBehaviourAction);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            enemyBehaviourRoutine = coroutineHelper.StartCoroutine(EnemyBehaviourRoutine(enemyBehaviourConfig.EnemyBehaviourActionData));
        }
        
        private IEnumerator EnemyBehaviourRoutine(List<EnemyBehaviourActionData> enemyBehaviourActionData)
        {
            while (EnemyState == EnemyState.Alive)
            {
                foreach (var actionData in enemyBehaviourActionData)
                {
                    if (behaviourActions.TryGetValue(actionData.GetEnemyBehaviourActionType(), out var behaviourAction))
                    {
                        behaviourAction.Enter(actionData);
                        yield return new WaitForSeconds(actionData.ActionDuration);
                        behaviourAction.Exit();
                    }
                }
            }
            
            enemyBehaviourRoutine = null;
        }

        private void StopEnemyBehaviourRoutine()
        {
            if (enemyBehaviourRoutine == null)
                return;
            
            coroutineHelper.StopCoroutine(enemyBehaviourRoutine);
            enemyBehaviourRoutine = null;
        }
        
        private void DispatchPlayerHit(int damage)
        {
            OnPlayerHit?.Invoke(damage);
        }
        #endregion
    }
}
