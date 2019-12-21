using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Configs.ConfigUtility;


#if UNITY_EDITOR
using UnityEditor;
#endif

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
namespace Utility
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AssetUri))]
    public class AssetUriDrawer : PropertyDrawer
    {
        public const string DefaultPath = "Assets/Resources/";
        public const char Seperator = '|'; 

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use AssetUri with string.");
                return;
            }

            var attr = attribute as AssetUri;
            var uri = ResolvePath(property.stringValue);
            var guid = ResolveGuid(property.stringValue);
            Object value = null;
            if (!string.IsNullOrEmpty(uri))
            {
                value = Resources.Load(uri);
            }
            else if (!string.IsNullOrEmpty(guid) && value == null)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                value = Resources.Load(assetPath);
            }
            property.stringValue = Resolve(EditorGUI.ObjectField(position, label, value, attr.Type, false));
        }

        public static string Field(string label, string uri, Type type)
        {
            Object value = null;
            if (!string.IsNullOrEmpty(uri))
            {
                value = Resources.Load(uri);
            }

            return Resolve(EditorGUILayout.ObjectField(label, value, type, false));
        }

        public static string Resolve(Object value)
        {
            if (value == null)
            {
                return null;
            }
            var assetPath = AssetDatabase.GetAssetPath(value);
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }
            if (!assetPath.StartsWith(DefaultPath))
            {
                throw new ArgumentException("Asset should be in " + DefaultPath);
            }

            var startIndex = DefaultPath.Length;
            var endIndex = assetPath.LastIndexOf('.');
            if (endIndex == -1)
            {
                throw new ArgumentException("Asset file name should end with .*");
            }

            var pathSubstring = assetPath.Substring(startIndex, endIndex - startIndex);
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            return guid + Seperator + pathSubstring;
        }

        public static string ResolvePath(string property)
        {
            var seperatorIndex = Array.IndexOf(property.ToCharArray(), Seperator);
            if (seperatorIndex == -1)
            {
                return property;
            }
            return property.Substring(seperatorIndex + 1);
        }

        public static string ResolveGuid(string property)
        {
            var seperatorIndex = Array.IndexOf(property.ToCharArray(), Seperator);
            if (seperatorIndex == -1)
            {
                return property;
            }
            return property.Substring(0, seperatorIndex - 1);
        }
    }
}
#endif