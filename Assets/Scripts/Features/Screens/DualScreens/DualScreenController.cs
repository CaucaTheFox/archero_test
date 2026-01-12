using UnityEngine;

namespace Features.Screens
{
    public abstract class DualScreenController<Screen2DType, Screen3DType>: IScreenController
        where Screen2DType: MonoBehaviour
        where Screen3DType: MonoBehaviour
    {
        #region Properties
        public abstract string Name { get; }
        
        public Screen2DType Screen2D { get; set; }
        public Screen3DType Screen3D { get; set; }

        public virtual bool Visible
        {
            get => Screen2D.gameObject.activeInHierarchy;
            set {
                Screen2D.gameObject.SetActive(value);
                Screen3D.gameObject.SetActive(value);
            }
        }
        #endregion
        
        #region Lifecycle
        public virtual void Init()
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }
        
        public void DestroyScreen()
        {
            OnDestroy();
            
            Object.Destroy(Screen2D.gameObject);
            Object.Destroy(Screen3D.gameObject);
        }
        #endregion
    }
}