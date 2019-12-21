using Configs.ConfigUtility;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    public class FixAssetUriPaths
    {
        private const string DefaultPath = "Assets/Resources/";
        private const char Seperator = '|'; 

        [MenuItem("Assets/Fix Asset Paths", false, 0)]
        public static void FixAssetPaths()
        {
            var configs = Resources.LoadAll("Configs");
            foreach (var config in configs)
            {
                var fields = config.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var fieldInfo in fields)
                {
                    object[] attributes = fieldInfo.GetCustomAttributes(true);
                    foreach (object attribute in attributes)
                    {
                        AssetUri assetUriAttribute = attribute as AssetUri;
                        if (assetUriAttribute == null)
                        {
                            continue;
                        }

                        var propertyValue = fieldInfo.GetValue(config) as string;
                        if (string.IsNullOrEmpty(propertyValue))
                        {
                            continue;
                        }

                        EditorUtility.SetDirty(config);
                        var newValue = UpdateAssetUriValue(propertyValue);
                        fieldInfo.SetValue(config, newValue);
                        Debug.Log($"Updated {config.name} field {fieldInfo.Name} : Old: {propertyValue}; New: {newValue}");
                    }
                }
            }

            AssetDatabase.SaveAssets();
        }

        private static string UpdateAssetUriValue(string stringValue)
        {
            var guid = ResolveGuid(stringValue);
            stringValue = ResolveBasedOnGuid(guid);
            return stringValue;
        }

        private static string ResolveBasedOnGuid(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
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
            return guid + Seperator + pathSubstring;
        }


        public static string ResolveGuid(string property)
        {
            var seperatorIndex = Array.IndexOf(property.ToCharArray(), Seperator);
            if (seperatorIndex == -1)
            {
                return property;
            }
            return property.Substring(0, seperatorIndex);
        }
    }
}