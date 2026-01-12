using UnityEngine;

namespace UI
{
    public class MarkerView : MonoBehaviour
    {
        #region State
        private Transform target;
        private Camera mainCamera;
        #endregion

        #region Lifecycle
        public void SetTarget(Transform target, Camera mainCamera)
        {
            this.target = target;
            this.mainCamera = mainCamera;
        }

        public void Update()
        {
            if (target == null)
                return;

            transform.position = GetScreenPosition;
        }
        #endregion

        #region Private
        private Vector3 GetScreenPosition => mainCamera.WorldToScreenPoint(target.position);
        #endregion
    }
}