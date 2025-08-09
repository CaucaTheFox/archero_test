using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.RenderFeatures
{
    public interface IRenderFeatureModel
    {
        void AddRendererForDepthTexture(Renderer renderer);
        void RemoveRendererForDepthTexture(Renderer renderer);
    }
    
    public class RenderFeatureModel: IRenderFeatureModel
    {
        #region State
        private RenderFeatureMediator renderFeatureMediator;
        #endregion

        #region Lifecycle
        private void Init()
        {
            renderFeatureMediator = Object.FindObjectOfType<RenderFeatureMediator>();
            renderFeatureMediator.Init();
        }
        #endregion
        
        #region Public
        public void AddRendererForDepthTexture(Renderer renderer)
        {
            renderFeatureMediator.AddRendererForDepthTexture(renderer);
        }

        public void RemoveRendererForDepthTexture(Renderer renderer)
        {
            renderFeatureMediator.RemoveRendererForDepthTexture(renderer);
        }
        
        public void ToggleBloomRenderFeature(bool enabled)
        {
            renderFeatureMediator.ToggleBloomRenderFeature(enabled);
        }
        #endregion
    }
}