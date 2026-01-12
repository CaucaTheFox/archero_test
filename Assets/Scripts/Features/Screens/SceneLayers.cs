using System;
using UnityEngine;

namespace Features.Screens
{
    public enum SceneLayer: int
    {
        ScreenLayer,
        HudLayer,
        WorldLayer
    }
    
    public interface ISceneLayers
    {
        Transform GetLayerTransform(SceneLayer gameLayer);
        bool IsAnyPopupOpen(SceneLayer gameLayer); 
    }
    
    public class SceneLayers: ISceneLayers
    {
        #region State
        private Transform[] layerTransforms;
        #endregion

        #region Lifecycle
        private void Init()
        {
            var names = Enum.GetNames(typeof(SceneLayer));
            layerTransforms = new Transform[names.Length];
        }
        #endregion
        
        public Transform GetLayerTransform(SceneLayer gameLayer)
        {
            var index = (int) gameLayer;

            var transform = layerTransforms[index];

            if (transform != null) {
                return transform;
            }
            
            var gameObject = GameObject.Find(gameLayer.ToString());
            
            if (gameObject == null) {
                return null;
            }
                
            return layerTransforms[index] = gameObject.transform;
        }

        public bool IsAnyPopupOpen(SceneLayer gameLayer)
        {
            var layerTransform = GetLayerTransform(gameLayer);
            return layerTransform.childCount > 0;
        }
    }
}