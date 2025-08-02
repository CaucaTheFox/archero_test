using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.CoroutineHelper
{
    public interface ICoroutineHelper
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
    }

    public class CoroutineHelper : ICoroutineHelper
    {
        private CoroutineHelperBehaviour coroutineHelper;

        public void Init()
        {
            var go = new GameObject { name = "CoroutineHelperBehaviour" };

            if(Application.isPlaying)
            {
                Object.DontDestroyOnLoad(go);
            }

            coroutineHelper = go.AddComponent<CoroutineHelperBehaviour>();
        }

        public void OnDestroy()
        {
            Object.Destroy(coroutineHelper.gameObject);
        }

        #region Public
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return coroutineHelper.StartCoroutine(routine);
        }
        
        public void StopCoroutine(Coroutine routine)
        {
           coroutineHelper.StopCoroutine(routine);
        }
        
        #endregion
    }
}