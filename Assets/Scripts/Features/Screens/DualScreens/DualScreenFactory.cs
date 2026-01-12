using Core.IoC;
using UnityEngine;

namespace Features.Screens
{
    public class DualScreenFactory<ScreenControllerType, Screen2DType, Screen3DType>: IScreenFactory
        where Screen2DType: MonoBehaviour
        where Screen3DType: MonoBehaviour
        where ScreenControllerType: DualScreenController<Screen2DType, Screen3DType>, new()
    {
        #region Dependencies
        [Inject] protected ISceneLayers gameLayers;
        [Inject] protected IControlsManager controlsManager;
        [Inject] protected IIoC container;
        #endregion
        
        #region State
        private string screen2DPrefabPath;
        private string screen3DPrefabPath;
        #endregion
        
        #region Lifecycle
        public DualScreenFactory(string screen2DPrefabPath, string screen3DPrefabPath)
        {
            this.screen2DPrefabPath = screen2DPrefabPath;
            this.screen3DPrefabPath = screen3DPrefabPath;
        }
        #endregion
        
        #region Public
        public IScreenController Create()
        {
            var layer3D = gameLayers.GetLayerTransform(SceneLayer.WorldLayer);
            var layer2D = gameLayers.GetLayerTransform(SceneLayer.ScreenLayer);
            
            var controller = new ScreenControllerType {
                Screen2D = controlsManager.Instantiate<Screen2DType>(screen2DPrefabPath, layer2D),
                Screen3D = controlsManager.Instantiate<Screen3DType>(screen3DPrefabPath, layer3D)
            };
            
            container.ResolveAll(controller);
            controller.Init();

            return controller;
        }
        #endregion
    }
}