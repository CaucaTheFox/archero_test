using UnityEngine;

namespace Core.CameraScripts
{
    public class TopDownCamera : MonoBehaviour
    {
        #region Unity Serialized Fields
        [SerializeField] private float height = 10f;
        [SerializeField] private float distance = 20f;
        [SerializeField] private float angle = 45f;
        [SerializeField] private float[] cameraXRotation;
        #endregion

        #region Properties
        public Transform CameraTarget{ get; set; }
        private Vector3 refVelocity;
        #endregion

        #region Lifecycle
        private void Update()
        {
            HandleCamera();
        }
        #endregion
        
        #region Private
        protected virtual void HandleCamera()
        {
            if (CameraTarget == null)
                return;
            

            var worldPosition = Vector3.forward * distance + Vector3.up * height;
            var rotatedWorldPosition = Quaternion.AngleAxis(angle, Vector3.up) * worldPosition;
            var targetPosition = CameraTarget.position;
            targetPosition.y = 0f;
            var newCameraPosition = targetPosition + rotatedWorldPosition;
            SetCameraPosition(newCameraPosition);
        }

        private void SetCameraPosition(Vector3 newPosition)
        {
            transform.position = newPosition;

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
