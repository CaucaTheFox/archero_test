#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utility
{
    public static class SerializedPropertyExtension
    {
        public static IEnumerable<SerializedProperty> GetVisibleChildren(this SerializedProperty serializedProperty)
        {
            var currentProperty = serializedProperty.Copy();
            var nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.NextVisible(false);
            }

            if(currentProperty.NextVisible(true))
            {
                do
                {
                    if(SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    yield return currentProperty.Copy();
                } while(currentProperty.NextVisible(false));
            }
        }

        #region Public
        //Code from: https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs lines 214-235 and 319-354
        //Found here: https://forum.unity.com/threads/get-a-general-object-value-from-serializedproperty.327098/#post-2309545
        public static object GetTargetObjectOfProperty(this SerializedProperty prop)
        {
            if(prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');

            foreach(var element in elements)
            {
                if(element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }
        
        public static int GetIndexOfTargetObject(this SerializedProperty prop)
        {
            if(prop == null) return -1;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            var indexList = new List<int>();
            foreach(var element in elements)
            {
                if (!element.Contains("[")) 
                    continue;
                
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                indexList.Add(index);
            }

            return indexList.Count == 0 ? -1 : indexList.Last();
        }

        private static object GetValue_Imp(object source, string name)
        {
            if(source == null)
                return null;
            var type = source.GetType();

            while(type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if(f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if(p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if(enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for(int i = 0; i <= index; i++)
            {
                if(!enm.MoveNext()) return null;
            }

            return enm.Current;
        }
        #endregion
    }
}
#endif