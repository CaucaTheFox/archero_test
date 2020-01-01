using Core.IoC;
using System.Collections;
using UnityEngine;

namespace Features.Enemies
{
    public class EnemyBehaviourProcessor : InjectableBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] protected Enemy enemy;
        [SerializeField] protected Behaviour[] behaviours;
        #endregion

        #region State
        private Coroutine processingRoutine;
        #endregion

        #region Lifecycle
        private void Start()
        {
            enemy.EnemyModel.OnDeath += HandleDeath;
            processingRoutine = StartCoroutine(ProcessBehaviourActions());
        }

        private void Update()
        {
            if (processingRoutine != null)
            {
                return;
            }

            processingRoutine = StartCoroutine(ProcessBehaviourActions());
        }
        #endregion

        #region Private     
        private IEnumerator ProcessBehaviourActions()
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.Action.Init(enemy);
                behaviour.Action.Enter();
                yield return new WaitForSeconds(behaviour.ActionDuration);
                behaviour.Action.Exit();
            }

            processingRoutine = null;
        }

        private void HandleDeath(int _)
        {
            enemy.EnemyModel.OnDeath -= HandleDeath;
            if (processingRoutine != null)
            {
                StopCoroutine(processingRoutine);
            }
        }
        #endregion
    }
}
