using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Core.RenderFeatures
{
    public class BlurRenderFeature : ScriptableRendererFeature
    {
        private BlurPass pass;
        public BlurPassSettings settings = new();

        public override void Create()
        {
            pass = new BlurPass(settings);
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
    public class BlurPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        [Range(1, 4)] public int Downsample = 1;
        [Range(0, 20)] public int BlurStrength = 5;
    }

    public class BlurPass : ScriptableRenderPass
    {
        #region Constants
        private static readonly int BlurStrengthProperty = Shader.PropertyToID("_BlurStrength");
        #endregion
        
        private BlurPassSettings passSettings;
        private RTHandle colorBuffer, temporaryBuffer;
        private Material material;

        public BlurPass(BlurPassSettings passSettings)
        {
            this.passSettings = passSettings;
            profilingSampler = new ProfilingSampler("BlurPass");
            renderPassEvent = passSettings.renderPassEvent;

            material = CoreUtils.CreateEngineMaterial("Hidden/Blur RenderFeature");
            material.SetInt(BlurStrengthProperty, passSettings.BlurStrength);
        }

        public void Dispose()
        {
            if (material != null)
                CoreUtils.Destroy(material);
            colorBuffer?.Release();
            temporaryBuffer?.Release();
        }
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.width /= passSettings.Downsample;
            descriptor.height /= passSettings.Downsample;
            descriptor.depthBufferBits = 0;
            descriptor.colorFormat = RenderTextureFormat.Default;

            RenderingUtils.ReAllocateIfNeeded(ref temporaryBuffer, descriptor, name: "_TemporaryBuffer");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
                return;
            
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
                Blitter.BlitCameraTexture(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, temporaryBuffer, material, 0);
                Blitter.BlitCameraTexture(cmd, temporaryBuffer, renderingData.cameraData.renderer.cameraColorTargetHandle, material, 1);
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }
}