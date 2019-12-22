using UnityEngine;

namespace CameraScripts
{
    public class TopDownCamera : MonoBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private float height = 10f;
        [SerializeField] private float distance = 20f;
        [SerializeField] private float angle = 45f;
        [SerializeField] private float smoothSpeed = 0.5f;
        #endregion

        #region Properties
        public Transform CameraTarget;/* { get; set; }*/
        private Vector3 refVelocity;
        #endregion
        #region Lifecycle
        private void Start()
        {
            HandleCamera();
        }
        private void Update()
        {
            HandleCamera();
        }
        #endregion

        #region Public
        #endregion

        #region Private
        protected virtual void HandleCamera()
        {
            if (CameraTarget == null)
            {
                return;
            }

            var worldPosition = Vector3.forward * distance + Vector3.up * height;
            var rotatedWorldPosition = Quaternion.AngleAxis(angle, Vector3.up) * worldPosition;

            var targetPosition = CameraTarget.position;
            targetPosition.y = 0f;
            var newCameraPosition = targetPosition + rotatedWorldPosition;
            transform.position = Vector3.SmoothDamp(
                transform.position,
                newCameraPosition, 
                ref refVelocity,
                smoothSpeed
            );
            transform.LookAt(CameraTarget);
        }
        #endregion
    }
}
