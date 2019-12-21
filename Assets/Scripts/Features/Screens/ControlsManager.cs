using Core.IoC;
using Core.ResourceManagement;
using UnityEngine;

namespace Features.Screens
{
    public interface IControlsManager
    {
        T Instantiate<T>(string prefabPath) where T : MonoBehaviour;
        T Instantiate<T>(string prefabPath, Transform parent) where T : MonoBehaviour;
        void Destroy(GameObject gameObject);
    }
    
    public class ControlsManager: IControlsManager
    {
        #region - Constants
        private const string LogTag = "PopupManager";
        #endregion
        
        #region - Dependencies
#pragma warning disable 649
        [Inject] private IResourceManager resourceManager;
#pragma warning restore 649
        #endregion
        
        #region - Public
        public T Instantiate<T>(string prefabPath) where T : MonoBehaviour
        {
            return Instantiate<T>(prefabPath, null);
        }
        
        public T Instantiate<T>(string prefabPath, Transform parent) where T: MonoBehaviour
        {
            var prefab = resourceManager.LoadResource<GameObject>(prefabPath);
            if (prefab == null) {
                return null;
            }

            var gameObject = GameObject.Instantiate(prefab, parent);

            if (gameObject == null) {
                return null;
            }

            gameObject.name = prefab.name;

            return gameObject.GetComponent<T>();
        }

        public void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
        #endregion
    }
}