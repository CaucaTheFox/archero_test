using Core.IoC;
using Features.Screens;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Core.RenderFeatures
{
    public class RenderFeatureMediator : InjectableBehaviour
    {
        #region Constants
        private static readonly int CustomDepthTextureDisabled = Shader.PropertyToID("CustomDepthTextureDisabled");
        #endregion
        
        #region Unity Serialized Fields
        [SerializeField] private UniversalRendererData screenRendererData;
        #endregion

        #region Dependencies
        [Inject] private IScreenManager screenManager;
        #endregion
        
        #region State
        private BloomRenderFeature bloomRenderFeature;
        private DepthTextureRenderFeature depthTextureRenderFeature;
        #endregion

        #region Public

        public void Init()
        {
            bloomRenderFeature = screenRendererData.rendererFeatures[0] as BloomRenderFeature;
            depthTextureRenderFeature = screenRendererData.rendererFeatures[1] as DepthTextureRenderFeature;
            Shader.SetGlobalInt(CustomDepthTextureDisabled, 0);
        }

        public void ToggleBloomRenderFeature(bool enable)
        {
            if(bloomRenderFeature == null)
                return;

            bloomRenderFeature.SetActive(enable);
        }
        
        public void AddRendererForDepthTexture(Renderer renderer)
        {
            depthTextureRenderFeature.AddRenderer(renderer);
        }

        public void RemoveRendererForDepthTexture(Renderer renderer)
        {
            depthTextureRenderFeature.RemoveRenderer(renderer);
        }
        #endregion
    }
}