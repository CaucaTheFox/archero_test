using Tools;
using UnityEditor;
using UnityEngine.UIElements;

namespace Features.Enemies
{
    [CustomPropertyDrawer(typeof(EnemyBehaviourActionData), true)]
    public class EnemyBehaviourActionDataPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var properties = EditorScriptUtility.CreatePropertyGUIForPropertiesExcluding(property);
            var foldOut = new Foldout { style = { marginBottom = 2 } };
            foldOut.Add(new VisualElement { style = { height = 5 } });

            foreach(var propertyField in properties)
            {
                foldOut.Add(propertyField);
            }

            return foldOut;
        }
    }
}