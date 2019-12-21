using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utility
{
    public class RaycastUtility : MonoBehaviour
    {
        #region Unity Serialized Fields

#pragma warning disable 649
// ReSharper disable NotNullMemberIsNotInitialized
        [SerializeField, NotNull] private Camera cam;
// ReSharper restore NotNullMemberIsNotInitialized
#pragma warning restore 649

        #endregion

        public GameObject Shoot()
        {
            if (IsPointerOverUi()) {
                return null;
            }
            
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hitInfo)) {
                return null;
            }
            
            return hitInfo.collider.gameObject;
        }

        public bool Shoot(out RaycastHit hit, LayerMask layerMask)
        {
            if (IsPointerOverUi()) {
                hit = default;
                return false;
            }

            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, layerMask)) {
                hit = hitInfo;
                return true;
            }
            
            hit = default;
            return false;
        }


        public bool IsPointerOverUi() =>
            (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) ||
            Input.touchCount > 0 && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
    }
}