using UnityEngine;

namespace Core.CameraScripts
{
    public class TopDownCamera : MonoBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private float height = 10f;
        [SerializeField] private float distance = 20f;
        [SerializeField] private float angle = 45f;
        [SerializeField] private float smoothSpeed = 0.5f;
        [SerializeField] private float[] xPositionBounds, zPositionBounds, cameraXRotation;
        #endregion

        #region Properties
        public Transform CameraTarget{ get; set; }
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
            EnsureCameraBounds(newCameraPosition);
        }

        private void EnsureCameraBounds(Vector3 newPosition)
        {
            var leftBounds = zPositionBounds[0];
            var rightBounds = zPositionBounds[1];
            var lowerBounds = xPositionBounds[0];
            var topBounds = xPositionBounds[1];

            newPosition.x = Mathf.Clamp(newPosition.x, lowerBounds, topBounds);
            newPosition.z = Mathf.Clamp(newPosition.z, leftBounds, rightBounds);

            transform.position = Vector3.SmoothDamp(
                transform.position,
                newPosition,
                ref refVelocity,
                smoothSpeed
            );

            transform.LookAt(CameraTarget);
            transform.rotation = Quaternion.Euler(
                Mathf.Clamp(transform.rotation.eulerAngles.x, cameraXRotation[0],cameraXRotation[1]),
                0,
                0
            );
        }
        #endregion
    }
}
