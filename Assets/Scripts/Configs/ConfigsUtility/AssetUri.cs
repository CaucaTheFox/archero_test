using System;
using UnityEngine;
using Object = UnityEngine.Object;

/**
 * Usage
 *
 * // Bind in default way
 * [AssetUri(typeof(Sprite))] public string SpriteUri;
 * 
 * // Bind in custom way
 * SpriteUri = (Sprite) AssetUriDrawer.Field("Select Sprite", SpriteUri, typeof(Sprite));
 * 
 * // Load resource at runtime
 * void Start() {
 *    var sprite = AssetUriLoader.Load<Sprite>(SpriteUri);
 * }
 */

namespace Configs.ConfigUtility
{
    public class AssetUri : PropertyAttribute
    {
        public readonly Type Type;

        public AssetUri(Type type)
        {
            Type = type ?? typeof(Object);
        }
    }
}