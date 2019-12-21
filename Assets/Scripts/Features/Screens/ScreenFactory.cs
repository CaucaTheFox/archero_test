using Core.IoC;
using UnityEngine;

namespace Features.Screens
{
    public class ScreenFactory<ScreenControllerType, ViewType>: IScreenFactory
        where ViewType: MonoBehaviour
        where ScreenControllerType: ScreenController<ViewType>, new()
    {
        #region - Dependencies
        [Inject] protected ISceneLayers gameLayers;
        [Inject] protected IControlsManager controlsManager;
        [Inject] protected IIoC container;
        #endregion
        
        #region - State
        private readonly string viewPrefabPath;
        #endregion
        
        #region - Lifecycle
        public ScreenFactory(string viewPrefabPath)
        {
            this.viewPrefabPath = viewPrefabPath;
        }
        #endregion
        
        #region - Public
        public IScreenController Create()
        {
            var screenLayer = gameLayers.GetLayerTransform(SceneLayer.ScreenLayer);
            
            var controller = new ScreenControllerType {
                View = controlsManager.Instantiate<ViewType>(viewPrefabPath, screenLayer)
            };
            
            container.ResolveAll(controller);
            controller.Init();

            return controller;
        }
        #endregion
    }
}