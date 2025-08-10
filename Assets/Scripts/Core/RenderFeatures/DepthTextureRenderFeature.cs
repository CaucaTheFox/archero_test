using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

namespace Core.RenderFeatures
{
    // creates a custom depth texture including the renderers assigned to it
    // useful if regular depth texture is not available (e.g. on mobile targets due to performance reasons)
    public class DepthTextureRenderFeature : ScriptableRendererFeature
    {
        public DepthTexturePassSettings settings = new();
        private DepthTexturePass pass;

        public override void Create()
        {
            if (pass != null)
                return;
            
            pass = new DepthTexturePass(settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }
 
        public void AddRenderer(Renderer renderer)
        {
            pass.AddRenderer(renderer);
        }

        public void RemoveRenderer(Renderer renderer)
        {
            pass.RemoveRenderer(renderer);
        }

        public void ClearRenderers()
        {
            pass.ClearRenderers();
        }

        protected override void Dispose(bool disposing)
        {
            ClearRenderers();
            pass?.Dispose();
            base.Dispose(disposing);
        }
    }
    
    [Serializable]
    public class DepthTexturePassSettings
    {
        public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public class DepthTexturePass : ScriptableRenderPass
    {
        #region Constants
        private const string ShaderPath = "Hidden/DepthTexture RenderFeature";
        private static readonly int ColorTex = Shader.PropertyToID("_ColorTex");
        private static readonly int MainTex = Shader.PropertyToID("_MainTex"); 
        #endregion
        
        #region State
        private DepthTexturePassSettings settings; 
        private Material depthMaterial;
        private Dictionary<Renderer, Material> rendererMaterialMap;

        private RTHandle depthTextureBuffer;
        #endregion
        
        #region Public
        public DepthTexturePass(DepthTexturePassSettings settings)
        {
            this.settings = settings;
            profilingSampler = new ProfilingSampler("DepthTexturePass");
            renderPassEvent = this.settings.RenderPassEvent;
            rendererMaterialMap ??= new Dictionary<Renderer, Material>();
        }

        public void Dispose()
        {
            if (rendererMaterialMap != null)
            {
                foreach (var material in rendererMaterialMap)
                {
                    CoreUtils.Destroy(material.Value);
                }
                rendererMaterialMap?.Clear();
            }
            if (depthMaterial != null)
                CoreUtils.Destroy(depthMaterial);
            depthTextureBuffer?.Release();
        }

        public void AddRenderer(Renderer renderer)
        {
            if (rendererMaterialMap.ContainsKey(renderer))
                return;

            if (depthMaterial == null)
            {
                depthMaterial = CoreUtils.CreateEngineMaterial(ShaderPath);
            }
            
            rendererMaterialMap.Add(renderer, depthMaterial);
        }

        public void RemoveRenderer(Renderer renderer)
        {
            rendererMaterialMap.Remove(renderer);
        }

        public void ClearRenderers()
        {
            rendererMaterialMap.Clear();
        }
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 32;
            descriptor.colorFormat = RenderTextureFormat.Depth;
            descriptor.graphicsFormat = GraphicsFormat.None;
            
            RenderingUtils.ReAllocateIfNeeded(ref depthTextureBuffer, descriptor, name: "_ColorBuffer");
            
            ConfigureTarget(depthTextureBuffer, depthTextureBuffer);
            ConfigureClear(ClearFlag.All, Color.black);
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                foreach (var (renderer, material) in rendererMaterialMap)
                {
                    if (renderer == null || !renderer.gameObject.activeInHierarchy || !renderer.enabled)
                    {
                        continue;
                    }

                    for (int i = 0; i < renderer.sharedMaterials.Count(); i++)
                    {
                        cmd.DrawRenderer(renderer, material, i);
                    }
                }
                
                cmd.SetGlobalTexture("_CustomDepthTexture", depthTextureBuffer);
              
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
        #endregion
    }
}