using System.Collections.Generic;
using UnityEngine;

namespace Utility.Utility
{
    public static class TransformUtility
    {
        public static void DestroyChildren(this Transform tr)
        {
            if (tr != null)
            {
                var list = new List<Transform>();
                foreach (Transform item in tr)
                    list.Add(item);

                for (var i = 0; i < list.Count; ++i)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        GameObject.DestroyImmediate(list[i].gameObject);
                    else
                        GameObject.Destroy(list[i].gameObject);
#else
                    GameObject.Destroy(list[i].gameObject);
#endif

                }
            }
            else
            {
                Debug.LogError("Transform is null");
            }
        }
        
        public static bool TryGetComponent<T>(this Transform transform, out T component) where T : Component
        {
            component = transform.GetComponent<T>();
            return component != null;
        }
    }
}