using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Core.RenderFeatures
{
    public class BloomRenderFeature : ScriptableRendererFeature
    {
        public BloomPassSettings settings = new();
        private BloomPass pass;

        public override void Create()
        {
            pass = new BloomPass(settings);
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isPreviewCamera)
                return;

            renderer.EnqueuePass(pass);
        }

        protected override void Dispose(bool disposing)
        {
            pass?.Dispose();
            base.Dispose(disposing);
        }
    }

    [Serializable]
    public class BloomPassSettings
    {
        public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public float Threshold = 1;
        public float SoftThreshold = 0.5f;
    }

    public class BloomPass : ScriptableRenderPass
    {
        #region Constants
        private static readonly int Filter = Shader.PropertyToID("_Filter");
        private static readonly int SourceTex = Shader.PropertyToID("_SourceTex");

        private const int BoxDownPrefilterPass = 0;
        private const int BoxDownPass = 1;
        private const int BoxUpPass = 2;
        private const int AdditiveBoxUpPass = 3;
        private const int ApplyBloomPass = 4;
        #endregion
        
        private Material bloomMaterial;

        private RTHandle temporaryBuffer1, temporaryBuffer2, colorBuffer;
        private Vector4 filter; 
        
        public BloomPass(BloomPassSettings settings)
        {
            profilingSampler = new ProfilingSampler("BloomPass");
            renderPassEvent = settings.RenderPassEvent;
            var knee = settings.Threshold * settings.SoftThreshold;
            filter.x = settings.Threshold;
            filter.y = filter.x - knee;
            filter.z = 2f * knee;
            filter.w = 0.25f / (knee + 0.00001f); 
            
            bloomMaterial = CoreUtils.CreateEngineMaterial("Hidden/Bloom RenderFeature");
            bloomMaterial.SetVector(Filter, filter);
        }

        public void Dispose()
        {
            if (bloomMaterial != null)
                CoreUtils.Destroy(bloomMaterial);
            temporaryBuffer1?.Release();
            temporaryBuffer2?.Release();
            colorBuffer?.Release();
        }
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            descriptor.colorFormat = RenderTextureFormat.Default;
            
            RenderingUtils.ReAllocateIfNeeded(ref colorBuffer, descriptor, name: "_ColorBuffer");
            
            descriptor.width /= 2;
            descriptor.height /= 2;
            
            RenderingUtils.ReAllocateIfNeeded(ref temporaryBuffer1, descriptor, name: "_TemporaryBuffer1");
            
            descriptor.width /= 2;
            descriptor.height /= 2;
            
            RenderingUtils.ReAllocateIfNeeded(ref temporaryBuffer2, descriptor, name: "_TemporaryBuffer2");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (bloomMaterial == null)
                return;
            
            #if UNITY_EDITOR
            bloomMaterial.SetVector(Filter, filter);
            #endif
            
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
                Blitter.BlitCameraTexture(cmd, source, temporaryBuffer1, bloomMaterial, BoxDownPrefilterPass);
                Blitter.BlitCameraTexture(cmd, temporaryBuffer1, temporaryBuffer2, bloomMaterial, BoxDownPass);
                Blitter.BlitCameraTexture(cmd, temporaryBuffer2, temporaryBuffer1, bloomMaterial, AdditiveBoxUpPass);
                cmd.SetGlobalTexture(SourceTex, temporaryBuffer1);
                
                Blitter.BlitCameraTexture(cmd, source, colorBuffer);
                Blitter.BlitCameraTexture(cmd, colorBuffer, renderingData.cameraData.renderer.cameraColorTargetHandle,
                    bloomMaterial, ApplyBloomPass);
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }
}