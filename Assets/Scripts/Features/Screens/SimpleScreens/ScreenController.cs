using UnityEngine;

namespace Features.Screens
{
    public abstract class ScreenController<ViewType>: IScreenController
        where ViewType: MonoBehaviour
    {
        #region Properties
        public abstract string Name { get; }
        public ViewType View { get; set; }

        public bool Visible
        {
            get => View.gameObject.activeInHierarchy;
            set => View.gameObject.SetActive(value);
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
            
            Object.Destroy(View.gameObject);
        }
        #endregion
    }
}