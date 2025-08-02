#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(SerializeReferenceButtonAttribute))]
public class SerializeReferenceButtonAttributeDrawer : PropertyDrawer
{
	private static readonly Color backgroundColor = new Color(0.75f, 0.75f, 0.75f, 1);

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if(string.IsNullOrEmpty(property.managedReferenceFullTypename) || !property.isExpanded)
		{
			return EditorGUI.GetPropertyHeight(property, true);
		}

		return EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		DrawSerializeReferenceButton(position, property, label);
		var propertyRect = new Rect(position.x, position.y, position.width, position.height);
		EditorGUI.PropertyField(propertyRect, property, GUIContent.none, true);
	}

	public override VisualElement CreatePropertyGUI(SerializedProperty property)
	{
		var container = new VisualElement { style = { minHeight = 21.0f } };
		var header = new VisualElement { style = { flexDirection = FlexDirection.Row, position = Position.Absolute } };
		var label = new Label(GetLabelText(property)) { style = { width = EditorGUIUtility.labelWidth * 2.0f } };
		var button = new Button { text = "Change Type", style = { width = 250.0f } };
		
		header.Add(label);
		header.Add(button);
		container.Add(header);

		var propertyField = new PropertyField() { label = " " };
		propertyField.BindProperty(property);
		container.Add(propertyField);
		header.BringToFront();

		button.clicked += () => ShowContextMenu(property, onUnbindProperty, onBindProperty);
	
		return container;

		void onUnbindProperty()
		{
			propertyField.Unbind();
		}

		void onBindProperty()
		{
			propertyField.BindProperty(property);
			label.text = GetLabelText(property);
		}
	}

	private string GetLabelText(SerializedProperty property)
	{
		var labelText = "";
		var referenceButtonAttribute = attribute as SerializeReferenceButtonAttribute;
		var elementIndex = "";

		if(referenceButtonAttribute?.showIndexInLabel ?? false)
		{
			var propertyPath = property.propertyPath;
			var data = propertyPath.Split('.')[^1];
			var isArrayElement = data.Contains("[");

			if(isArrayElement)
			{
				var lastIndex = data.LastIndexOf('[');
				elementIndex = data.Substring(lastIndex + 1, data.Length - 1 - lastIndex - 1);
			}
		}

		if(string.IsNullOrEmpty(property.managedReferenceFullTypename))
		{
			if(!string.IsNullOrEmpty(elementIndex))
			{
				labelText = $"{elementIndex} - {property.displayName}";
			}
		}
		else
		{
			labelText = GetType(property.managedReferenceFullTypename).Name;
			if(!string.IsNullOrEmpty(elementIndex))
			{
				labelText = $"{elementIndex} - {labelText}";
			}
		}

		return labelText;
	}

	protected void DrawSerializeReferenceButton(Rect position, SerializedProperty property, GUIContent label)
	{
		var labelPosition = new Rect(position.x + 15, position.y, position.width, EditorGUIUtility.singleLineHeight);
		var referenceButtonAttribute = attribute as SerializeReferenceButtonAttribute;
		var elementIndex = "";

		if(referenceButtonAttribute?.showIndexInLabel ?? false)
		{
			var propertyPath = property.propertyPath;
			var data = propertyPath.Split('.')[^1];
			var isArrayElement = data.Contains("[");

			if(isArrayElement)
			{
				var lastIndex = data.LastIndexOf('[');
				elementIndex = data.Substring(lastIndex + 1, data.Length - 1 - lastIndex - 1);
			}
		}

		if(string.IsNullOrEmpty(property.managedReferenceFullTypename))
		{
			if(!string.IsNullOrEmpty(elementIndex))
			{
				label.text = $"{elementIndex} - {label.text}";
			}
			EditorGUI.LabelField(labelPosition, label);
			DrawSelectionButton(property, position);
		}
		else if(property.isExpanded)
		{
			var typeLabel = GetType(property.managedReferenceFullTypename).Name;
			if(!string.IsNullOrEmpty(elementIndex))
			{
				typeLabel = $"{elementIndex} - {typeLabel}";
			}
			EditorGUI.LabelField(labelPosition, typeLabel);
			var rect = new Rect(position.x + EditorGUI.indentLevel * 15, labelPosition.y, position.width / 1.5f, position.height);
			DrawSelectionButton(property, rect);
		}
		else
		{
			var typeLabel = GetType(property.managedReferenceFullTypename).Name;
			if(!string.IsNullOrEmpty(elementIndex))
			{
				typeLabel = $"{elementIndex} - {typeLabel}";
			}
			EditorGUI.LabelField(labelPosition, typeLabel);
			var rect = new Rect(position.x + EditorGUI.indentLevel * 15, labelPosition.y, position.width / 1.5f, position.height);
			DrawSelectionButton(property, rect);
		}
	}

	private void DrawSelectionButton(SerializedProperty property, Rect position)
	{
		var buttonPosition = position;
		buttonPosition.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
		buttonPosition.width = position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;
		buttonPosition.height = EditorGUIUtility.singleLineHeight;

		var previousIndentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		var previousBackgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = backgroundColor;

		if(GUI.Button(buttonPosition, new GUIContent("Change Type")))
		{
			ShowContextMenu(property);
		}

		GUI.backgroundColor = previousBackgroundColor;
		EditorGUI.indentLevel = previousIndentLevel;
	}

	private void ShowContextMenu(SerializedProperty property, Action onUnbindProperty = null, Action onBindProperty = null)
	{
		var contextMenu = new GenericMenu();
		FillContextMenu(contextMenu, property, onUnbindProperty, onBindProperty);
		contextMenu.ShowAsContext();
	}

	private void FillContextMenu(GenericMenu contextMenu, SerializedProperty property, Action onUnbindProperty = null, Action onBindProperty = null)
	{
		var types = GetDerivedTypes(property);

		foreach(var type in types)
		{
			contextMenu.AddItem(new GUIContent(type.Name), false, SetManagedReferenceToNewInstance, (type, property, onUnbindProperty, onBindProperty));
		}
	}

	private void SetManagedReferenceToNull(object userData)
	{
		var serializedProperty = (SerializedProperty)userData;

		serializedProperty.serializedObject.Update();
		serializedProperty.managedReferenceValue = null;
		serializedProperty.serializedObject.ApplyModifiedProperties();
	}

	private void SetManagedReferenceToNewInstance(object userData)
	{
		var (type, property, onUnbindProperty, onBindProperty) = ((Type type, SerializedProperty property, Action onUnbindProperty, Action onBindProperty))userData;

		if(type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null)
		{
			var serializedProperty = property;
			var instance = Activator.CreateInstance(type);

			onUnbindProperty?.Invoke();
			serializedProperty.serializedObject.Update();
			serializedProperty.managedReferenceValue = instance;
			serializedProperty.serializedObject.ApplyModifiedProperties();
			onBindProperty?.Invoke();
		}
		else
		{
			Debug.LogWarning($"Failed to create an instance of type \'{type}\' because the type does not provide a parameterless constructor.");
		}
	}

	private IEnumerable<Type> GetDerivedTypes(SerializedProperty property)
	{
		var type = GetType(property.managedReferenceFieldTypename);

		if(type == null)
		{
			yield break;
		}

		var derivedTypes = TypeCache.GetTypesDerivedFrom(type)
			.Where(derivedType => !derivedType.IsSubclassOf(typeof(Object))
			                      && !derivedType.IsAbstract).ToList();
		var allowedTypes = new List<Type>();
		var typeFilters = SerializedReferenceUIDefaultTypeRestrictions.GetAllBuiltInTypeRestrictions(fieldInfo).ToList();

		if(typeFilters.Count == 0)
			allowedTypes.AddRange(derivedTypes);
		else
		{
			foreach(var filter in typeFilters.Where(filter => filter != null))
			{
				allowedTypes.AddRange(derivedTypes.Where(derivedType => filter.Invoke(derivedType)));
			}
		}

		foreach(var allowedType in allowedTypes)
		{
			yield return allowedType;
		}
	}

	private Type GetType(string fullName)
	{
		var typeName = fullName;

		var typeSplitString = typeName.Split(char.Parse(" "));
		var typeClassName = typeSplitString[1];
		var typeAssemblyName = typeSplitString[0];
		var type = Type.GetType($"{typeClassName}, {typeAssemblyName}");
		return type;
	}
}
#endif