using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Core.RenderFeatures
{
    public class DepthOfFieldRenderFeature : ScriptableRendererFeature
    {
        private DepthOfFieldPass pass;
        public DepthOfFieldPassSettings settings = new();

        public override void Create()
        {
            pass = new DepthOfFieldPass(settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }

        protected override void Dispose(bool disposing)
        {
            pass?.Dispose();
            base.Dispose(disposing);
        }
    }

    [Serializable]
    public class DepthOfFieldPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        public bool VisualizeLens; 
        [Range(1, 4)] public int Downsample = 1;
        [Range(0f, 10f)] public float BlurStrength = 5;
        public float Focus = 1;
        public float Aperture = 1;
    }

    public class DepthOfFieldPass : ScriptableRenderPass
    {
        #region Constants
        private static readonly int VisualizeLensProperty = Shader.PropertyToID("_VisualizeLens");
        private static readonly int BlurStrengthProperty = Shader.PropertyToID("_BlurStrength");
        private static readonly int FocusProperty = Shader.PropertyToID("_Focus");
        private static readonly int ApertureProperty = Shader.PropertyToID("_Aperture");
        #endregion
        
        private DepthOfFieldPassSettings passSettings;
        private RTHandle temporaryBuffer1, temporaryBuffer2, colorBuffer;
        private Material material;

        public DepthOfFieldPass(DepthOfFieldPassSettings passSettings)
        {
            this.passSettings = passSettings;
            profilingSampler = new ProfilingSampler("DepthOfFieldPass");
            renderPassEvent = passSettings.renderPassEvent;

            material = CoreUtils.CreateEngineMaterial("Hidden/DepthOfField RenderFeature");
            SetMaterialValues();
        }

        public void Dispose()
        {
            if (material != null)
                CoreUtils.Destroy(material);
            temporaryBuffer1?.Release();
            temporaryBuffer2?.Release();
            colorBuffer?.Release();
        }
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            descriptor.colorFormat = RenderTextureFormat.Default;
            
            var downSampledDescriptor = descriptor;
            downSampledDescriptor.width /= passSettings.Downsample;
            downSampledDescriptor.height /= passSettings.Downsample;
            
            RenderingUtils.ReAllocateIfNeeded(ref colorBuffer, descriptor, name: "_ColorBuffer");
            RenderingUtils.ReAllocateIfNeeded(ref temporaryBuffer1, downSampledDescriptor, name: "_TemporaryBuffer1");
            RenderingUtils.ReAllocateIfNeeded(ref temporaryBuffer2, downSampledDescriptor, name: "_TemporaryBuffer2");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
                return;
            
            
            SetMaterialValues();
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
                Blitter.BlitCameraTexture(cmd, source, temporaryBuffer1, material, 0); 
                Blitter.BlitCameraTexture(cmd, temporaryBuffer1, temporaryBuffer2, material, 0); 
                cmd.SetGlobalTexture("_BlurTex", temporaryBuffer2); 
                Blitter.BlitCameraTexture(cmd, source, colorBuffer);
                Blitter.BlitCameraTexture(cmd, colorBuffer, renderingData.cameraData.renderer.cameraColorTargetHandle, material, 1); 
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
        
        private void SetMaterialValues()
        {
            material.SetInt(VisualizeLensProperty, passSettings.VisualizeLens? 1: 0);
            material.SetFloat(BlurStrengthProperty, passSettings.BlurStrength);
            material.SetFloat(FocusProperty, passSettings.Focus);
            material.SetFloat(ApertureProperty, passSettings.Aperture);
        }
    }
}