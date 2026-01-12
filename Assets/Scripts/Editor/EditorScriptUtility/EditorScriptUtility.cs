using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;
using WanzyeeStudio.Json;
using ColorUtility = UnityEngine.ColorUtility;
using Object = UnityEngine.Object;

namespace Tools
{
    public static class EditorScriptUtility
    {
	    #region Public
        public static T LoadFileFromDirectory<T>(string directory, string configName)
        {
            var filePath = Path.Combine(directory, configName);
            if (!File.Exists(filePath))
            {
                return default;
            }

            var data = File.ReadAllText(filePath);
            var config = Deserialize<T>(data);
            return config;
        }
        
        public static List<T> LoadAssetsAtPath<T>(string[] foldersToSearch, string filter, bool matchFilterExactly = false) where T : Object
        {
            var guids = AssetDatabase.FindAssets(filter, foldersToSearch);
            var assets = new List<T>();
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);

                if(matchFilterExactly)
                {
                    var splitPath = path.Split('/');
                    var assetName = splitPath.Last().Split('.')[0];

                    if(assetName == filter)
                    {
                        assets.Add(AssetDatabase.LoadAssetAtPath<T>(path));
                    }
                }
                else
                {
                    assets.Add(AssetDatabase.LoadAssetAtPath<T>(path));
                }
            }

            return assets.Where(x => x != null).ToList();
        }

        public static List<string> GetAssetLocations<T>(string[] foldersToSearch, string filter, 
	        bool matchFilterExactly = false, bool includeSubFolders = false) where T : Object
        {
            var guids = AssetDatabase.FindAssets(filter, foldersToSearch).ToList();
            if (includeSubFolders)
            {
	            var subFolders = foldersToSearch.SelectMany(AssetDatabase.GetSubFolders).ToArray();
	            guids.AddRange(AssetDatabase.FindAssets(filter, subFolders).ToList());
	            guids = guids.Distinct().ToList();
            }
            var paths = new List<string>();

            for(int i = 0; i < guids.Count; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if(matchFilterExactly)
                {
                    var splitPath = path.Split('/');
                    var assetName = splitPath.Last().Split('.')[0];

                    if(assetName == filter && AssetDatabase.LoadAssetAtPath<T>(path) != null)
                    {
                        paths.Add(path);
                    }
                }
                else
                {
                    if(AssetDatabase.LoadAssetAtPath<T>(path) != null)
                    {
                        paths.Add(path);
                    }
                }
            }

            return paths;
        }

        public static void SaveConfig<T>(T config, string path)
        {
            var json = Serialize(config);
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }

        public static void UIDividerHorizontal(Color color, int thickness = 2, int paddingY = 10, int paddingX = 10, float maxWidth = -1)
        {
	        var r = maxWidth >= 0.0f
		        ? EditorGUILayout.GetControlRect(GUILayout.Height(paddingY + thickness), GUILayout.MaxWidth(maxWidth))
		        : EditorGUILayout.GetControlRect(GUILayout.Height(paddingY + thickness));
            r.height = thickness;
            r.y += paddingY * 0.5f;
            r.x += paddingX;
            r.width -= paddingX * 2;
            EditorGUI.DrawRect(r, color);
        }

        public static void UIDividerVertical(Color color, float height, int thickness = 2, int paddingY = 5, int paddingX = 10)
        {
            var r = EditorGUILayout.GetControlRect(GUILayout.Width(paddingX + thickness));
            r.width = thickness;
            r.y += paddingY;
            r.x += paddingX * 0.5f;
            r.height = height - paddingY * 2;
            EditorGUI.DrawRect(r, color);
        }

        public static VisualElement CreateUIDividerHorizontal(Color color, int thickness = 2, int paddingY = 10, int paddingX = 5, float maxWidth = -1)
        {
	        var divider = new VisualElement
	        {
		        style =
		        {
			        backgroundColor = color,
			        height = thickness,
			        marginLeft = paddingX,
			        marginRight = paddingX,
			        marginTop = paddingY,
			        marginBottom = paddingY
		        }
	        };

	        if(maxWidth >= 0)
	        {
		        divider.style.maxWidth = maxWidth;
	        }
	        
	        return divider;
        }

        public static string GetNumericIdTwoDigits(int numericId)
        {
            var numericString = numericId < 10
                ? $"0{numericId}"
                : numericId.ToString();
            return $"{numericString}"; 
        }
        
        public static string GetNumericIdThreeDigits(int numericId)
        {
            var numericString = numericId < 10 
                ? $"00{numericId}" 
                : numericId < 100 
                    ? $"0{numericId}" 
                    : numericId.ToString();
            return $"{numericString}"; 
        }

        /// <summary>
        /// Draws an array with uneditable size and ordering.
        /// <para>Use this version for Custom Inspector</para>
        /// </summary>
        /// <returns>Is the foldout expanded or not</returns>
        public static bool DrawUneditableArrayWithFoldOut(SerializedProperty array, bool foldoutExpanded, string label = null)
        {
            var lastExpand = foldoutExpanded;
            foldoutExpanded = EditorGUILayout.Foldout(foldoutExpanded, string.IsNullOrEmpty(label) ? array.displayName : label, true);

            if(foldoutExpanded != lastExpand)
            {
                if(Event.current.alt)
                {
                    for(int i = 0; i < array.arraySize; i++)
                    {
                        var arrayElement = array.GetArrayElementAtIndex(i);
                        arrayElement.isExpanded = foldoutExpanded;
                        CollapseAllChildProperties(arrayElement, foldoutExpanded);
                    }
                }
            }

            if(foldoutExpanded)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;

                for(int i = 0; i < array.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i));
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }

            return foldoutExpanded;
        }

        /// <summary>
        /// Draws an array with uneditable size and ordering.
        /// <para>Use this version for Custom Property Drawer</para>
        /// </summary>
        /// <returns>Is the foldout expanded or not</returns>
        public static bool DrawUneditableArrayWithFoldOut(Rect position, SerializedProperty array, bool foldoutExpanded, 
	        string label = null, bool useFoldoutHeaderGroup = false)
        {
            return DrawUneditableArrayWithFoldOut(position, array, foldoutExpanded, out _, label, useFoldoutHeaderGroup);
        }

        /// <summary>
        /// Draws an array with uneditable size and ordering.
        /// <para>Use this version for Custom Property Drawer</para>
        /// </summary>
        /// <returns>Is the foldout expanded or not</returns>
        public static bool DrawUneditableArrayWithFoldOut(Rect position, SerializedProperty array, bool foldoutExpanded, 
	        out float height, string label = null, bool useFoldoutHeaderGroup = false)
        {
            var lastExpand = foldoutExpanded;

            if(useFoldoutHeaderGroup)
            {
                var indentedPosition = EditorGUI.IndentedRect(position);
                foldoutExpanded = EditorGUI.BeginFoldoutHeaderGroup(indentedPosition, foldoutExpanded, 
	                string.IsNullOrEmpty(label) ? array.displayName : label);
            }
            else
            {
                foldoutExpanded = EditorGUI.Foldout(position, foldoutExpanded,
	                string.IsNullOrEmpty(label) ? array.displayName : label, true);
            }

            if(foldoutExpanded != lastExpand)
            {
                if(Event.current.alt)
                {
                    for(int i = 0; i < array.arraySize; i++)
                    {
                        var arrayElement = array.GetArrayElementAtIndex(i);
                        arrayElement.isExpanded = foldoutExpanded;
                        CollapseAllChildProperties(arrayElement, foldoutExpanded);
                    }
                }
            }
            
            height = 0.0f;
            
            if(foldoutExpanded)
            {
                for(int i = 0; i < array.arraySize; i++)
                {
                    var arrayElement = array.GetArrayElementAtIndex(i);
                    height += EditorGUI.GetPropertyHeight(arrayElement, true) + EditorGUIUtility.standardVerticalSpacing;
                }
                height += EditorGUIUtility.standardVerticalSpacing;

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                position.height = height;

                if(EditorGUI.indentLevel == 0)
                {
                    EditorGUI.indentLevel++;
                    position = EditorGUI.IndentedRect(position);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    position = EditorGUI.IndentedRect(position);
                }
                EditorGUI.HelpBox(position, "", MessageType.None);

                for(int i = 0; i < array.arraySize; i++)
                {
                    var arrayElement = array.GetArrayElementAtIndex(i);
                    position.height = EditorGUI.GetPropertyHeight(arrayElement, true);
                    EditorGUI.PropertyField(position, array.GetArrayElementAtIndex(i), true);
                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            
            height += EditorGUIUtility.singleLineHeight;
            
            if(useFoldoutHeaderGroup)
            {
                EditorGUI.EndFoldoutHeaderGroup();
            }
            
            return foldoutExpanded;
        }

        public static VisualElement CreateUneditableArrayGUIWithFoldOut(SerializedProperty array, bool foldOutExpanded, string label = null)
        {
	        var container = new Foldout
	        {
		        text = label ?? "",
		        value = foldOutExpanded
	        };

	        ColorUtility.TryParseHtmlString(EditorGUIUtility.isProSkin ? "#242424" : "#7F7F7F", out var borderColor);
	        ColorUtility.TryParseHtmlString(EditorGUIUtility.isProSkin ? "#414141" : "#C1C1C1", out var backGroundColor);
	        var background = new VisualElement
	        {
		        name = "PropertyContainer",
		        style =
		        {
			        borderLeftColor = borderColor,
			        borderRightColor = borderColor,
			        borderTopColor = borderColor,
			        borderBottomColor = borderColor,
			        borderTopLeftRadius = 3,
			        borderBottomLeftRadius = 3,
			        borderTopRightRadius = 3,
			        borderBottomRightRadius = 3,
			        borderLeftWidth = 1,
			        borderRightWidth = 1,
			        borderTopWidth = 1,
			        borderBottomWidth = 1,
			        backgroundColor = backGroundColor
		        }
	        };
	        container.Add(background);

	        var emptyArrayLabel = new Label
	        {
		        name = "EmptyArrayLabel",
		        text = "List contains no elements",
		        style = { display = array.arraySize == 0 ? DisplayStyle.Flex : DisplayStyle.None }
	        };
	        background.Add(emptyArrayLabel);

	        for(int i = 0; i < array.arraySize; i++)
	        {
		        var arrayElement = array.GetArrayElementAtIndex(i);
		        var propertyField = new PropertyField();
		        propertyField.BindProperty(arrayElement);

		        background.Add(propertyField);
	        }

	        return container;
        }

        public static VisualElement CreateArrayGUIWithFoldOut(SerializedProperty array, bool foldoutExpanded, string label = null,
            Action<VisualElement> handleAddElementButtonClicked = null, Action<VisualElement> handleRemoveElementButtonClicked = null)
        {
	        var arrayGUI = CreateUneditableArrayGUIWithFoldOut(array, foldoutExpanded, label);

	        var buttonContainer = new VisualElement()
	        {
		        style =
		        {
			        flexDirection = FlexDirection.RowReverse,
		        }
	        };
	        var removeButton = new Button()
	        {
		        text = "-",
		        style = { maxWidth = 50 }
	        };
	        removeButton.clicked += () =>
	        {
		        handleRemoveElementButtonClicked?.Invoke(arrayGUI.Q<VisualElement>("PropertyContainer"));
		        var emptyArrayLabel = arrayGUI.Q("EmptyArrayLabel");
		        emptyArrayLabel.style.display = array.arraySize == 0 ? DisplayStyle.Flex : DisplayStyle.None;
	        };
	        buttonContainer.Add(removeButton);
	        var addButton = new Button()
	        {
		        text = "+",
		        style = { maxWidth = 50 }
	        };
	        addButton.clicked += () =>
	        {
		        handleAddElementButtonClicked?.Invoke(arrayGUI.Q<VisualElement>("PropertyContainer"));
		        var emptyArrayLabel = arrayGUI.Q("EmptyArrayLabel");
		        emptyArrayLabel.style.display = array.arraySize == 0 ? DisplayStyle.Flex : DisplayStyle.None;
	        };
	        buttonContainer.Add(addButton);
	        arrayGUI.Add(buttonContainer);

	        return arrayGUI;
        }

        public static Rect DrawVisiblePropertiesExcluding(Rect position, SerializedProperty property, params string[] propertyToExclude)
        {
            var propertiesToDraw = property.GetVisibleChildren().Where(x => !propertyToExclude.Contains(x.name));

            foreach(var prop in propertiesToDraw)
            {
                position.height = EditorGUI.GetPropertyHeight(prop, true);
                EditorGUI.PropertyField(position, prop, true);
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            }

            return position;
        }

        public static List<VisualElement> CreatePropertyGUIForPropertiesExcluding(SerializedProperty property, params string[] propertyToExclude)
        {
	        var propertiesToDraw = property.GetVisibleChildren().Where(x => !propertyToExclude.Contains(x.name));
	        var propertyFields = CreatePropertyFields(propertiesToDraw);
		        
	        return propertyFields;
        }

        public static List<VisualElement> CreatePropertyGUIForPropertiesExcluding(SerializedObject serializedObject, params string[] propertyToExclude)
        {
	        var property = serializedObject.GetIterator();
	        var propertyFields = new List<VisualElement>();
	        var isFirstPass = true;

	        while(property.NextVisible(isFirstPass))
	        {
		        isFirstPass = false;
		        if(propertyToExclude.Contains(property.name))
			        continue;
		        
		        var propertyField = new PropertyField()
		        {
			        name = property.name
		        };
		        propertyField.BindProperty(property);
		        propertyFields.Add(propertyField);
	        }

	        return propertyFields;
        }

        public static List<VisualElement> CreatePropertyGUIForType<T>(SerializedProperty property, params string[] propertyToExclude)
        {
	        var type = typeof(T);
	        var typeFields = type.GetFields(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public 
	                                        | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
	        var visibleFieldNames = new List<string>();

	        foreach(var field in typeFields)
	        {
				var attributes = field.GetCustomAttributes(false);
				
				if((field.IsPublic || attributes.Any(x => x is SerializeField)) && (!propertyToExclude.Contains(field.Name) 
					   || attributes.Any(x => x is HideInInspector)))
					visibleFieldNames.Add(field.Name);
	        }
         
	        var propertiesToDraw = property.GetVisibleChildren().Where(x => visibleFieldNames.Contains(x.name));
	        var propertyFields = CreatePropertyFields(propertiesToDraw);
            
	        return propertyFields;
        }

        private static List<VisualElement> CreatePropertyFields(IEnumerable<SerializedProperty> propertiesToDraw)
        {
	        var propertyFields = new List<VisualElement>();

	        foreach(var prop in propertiesToDraw)
	        {
		        var propertyField = new PropertyField()
		        {
			        name = prop.name
		        };
		        propertyField.BindProperty(prop);
		        propertyFields.Add(propertyField);
	        }

	        return propertyFields;
        }
        
        public static PropertyField CreatePropertyField(SerializedProperty serializedProperty, string label,
	        DisplayStyle displayStyle, float opacity = 1.0f, bool setEnabled = true)
        {
	        var propertyField = new PropertyField()
	        {
		        name = serializedProperty.name,
		        label = label,
		        style =
		        {
			        display = displayStyle,
			        opacity = opacity
		        }
	        };
	        propertyField.BindProperty(serializedProperty);
	        propertyField.SetEnabled(setEnabled);
	        return propertyField;
        }
        
        private static void CollapseAllChildProperties(SerializedProperty property, bool isExpanded)
        {
            property.isExpanded = isExpanded;
            if(!property.hasVisibleChildren) return;

            var visibleChildren = property.GetVisibleChildren();

            foreach(var prop in visibleChildren)
            {
                CollapseAllChildProperties(prop, isExpanded);
            }
        }
        
        public static int GetArrayIndexOfSerializedProperty(SerializedProperty property)
        {
            var path = property.propertyPath;

            var start = path.LastIndexOf("[") + 1;
            var len = path.LastIndexOf("]") - start;

            if(len < 1)
                return -1;

            var index = -1;

            if((len > 0 && int.TryParse(path.Substring(path.LastIndexOf("[") + 1, len), out index)) == false)
            {
                Debug.Log("Attempted to find the index of a non-array serialized property.");
            }

            return index;
        }

        public static void RegisterArraySizeChangedCallback(SerializedProperty arrayProperty, VisualElement container,
	        EventCallback<ChangeEvent<int>> callback)
        {
	        if(!arrayProperty.isArray)
	        {
		        Debug.LogError("Trying to register array size change callback for property that is not an array!");
		        return;
	        }

	        var callbackHelperContainer = container.Q<VisualElement>("CallbackHelperContainer")
	                                      ?? new VisualElement { name = "CallbackHelperContainer" };
	        var size = arrayProperty.FindPropertyRelative("Array.size");
	        var propertyField = new PropertyField { style = { display = DisplayStyle.None } };
	        propertyField.BindProperty(size);
	        propertyField.RegisterCallback(callback);
	        callbackHelperContainer.Add(propertyField);
	        container.Add(callbackHelperContainer);
        }

        public static void RegisterPropertyChangedCallback(SerializedProperty property, VisualElement container, 
	        EventCallback<SerializedPropertyChangeEvent> callback)
        {
	        var callbackHelperContainer = container.Q<VisualElement>("CallbackHelperContainer")
	                                      ?? new VisualElement { name = "CallbackHelperContainer" };
	        var propertyField = new PropertyField { style = { display = DisplayStyle.None } };
	        propertyField.BindProperty(property);
	        propertyField.RegisterValueChangeCallback(callback);
	        callbackHelperContainer.Add(propertyField);
	        container.Add(callbackHelperContainer);
        }
        
		public static VisualElement CreateIdSelectionGUI(List<string> dropdownOptions, int selectedIndex,
			Action<ChangeEvent<string>, DropdownField> onSelectedIdChanged,
			Func<string, (string, string)> getUpdatedButtonLabelFunction,
			Action<DropdownField> onPreviousIdButtonClicked,
			Action<DropdownField> onNextIdButtonClicked)
		{
			var idSelectionContainerTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/EditorScriptUtility/IdSelectionContainer.uxml");
			var idSelectionContainer = idSelectionContainerTemplate.Instantiate();

			var previousIdButton = idSelectionContainer.Q<Button>("PreviousIdButton");

			var idDropdown = idSelectionContainer.Q<DropdownField>("IdDropdown");
			idDropdown.choices = dropdownOptions;
			idDropdown.index = selectedIndex;

			var nextIdButton = idSelectionContainer.Q<Button>("NextIdButton");

			idDropdown.RegisterValueChangedCallback(HandleSelectedIdChanged);
			previousIdButton.clicked += () => onPreviousIdButtonClicked?.Invoke(idDropdown);
			nextIdButton.clicked += () => onNextIdButtonClicked?.Invoke(idDropdown);

			UpdateButtonLabels();

			return idSelectionContainer;

			void HandleSelectedIdChanged(ChangeEvent<string> evt)
			{
				onSelectedIdChanged?.Invoke(evt, idDropdown);
				UpdateButtonLabels();
			}

			void UpdateButtonLabels()
			{
				if(getUpdatedButtonLabelFunction == null)
					return;

				var (previousButtonText, nextButtonText) = getUpdatedButtonLabelFunction(dropdownOptions[selectedIndex]);
				previousIdButton.text = previousButtonText;
				nextIdButton.text = nextButtonText;
			}
		}

		public static void InitIdSelectionContainer(VisualElement idSelectionContainer,
			List<string> dropdownOptions,
			Action<ChangeEvent<string>, DropdownField> onSelectedIdChanged,
			Func<string, (string, string)> getUpdatedButtonLabelFunction,
			Action<DropdownField> onPreviousIdButtonClicked,
			Action<DropdownField> onNextIdButtonClicked)
		{
			var previousIdButton = idSelectionContainer.Q<Button>("PreviousIdButton");

			var idDropdown = idSelectionContainer.Q<DropdownField>("IdDropdown");
			idDropdown.choices = dropdownOptions;
			idDropdown.index = 0;

			var nextIdButton = idSelectionContainer.Q<Button>("NextIdButton");

			idDropdown.RegisterValueChangedCallback(HandleSelectedIdChanged);
			previousIdButton.clicked += () => onPreviousIdButtonClicked?.Invoke(idDropdown);
			nextIdButton.clicked += () => onNextIdButtonClicked?.Invoke(idDropdown);

			UpdateButtonLabels();

			void HandleSelectedIdChanged(ChangeEvent<string> evt)
			{
				onSelectedIdChanged?.Invoke(evt, idDropdown);
				UpdateButtonLabels();
			}

			void UpdateButtonLabels()
			{
				if(getUpdatedButtonLabelFunction == null)
					return;

				var (previousButtonText, nextButtonText) = getUpdatedButtonLabelFunction(idDropdown.choices[idDropdown.index]);
				previousIdButton.text = previousButtonText;
				nextIdButton.text = nextButtonText;
			}
		}
		
		public static List<VisualElement> GetAllChildVisualElementsRecursive(VisualElement element)
		{
			var result = new List<VisualElement>();
			var foldouts = element.Query<VisualElement>().ToList();
			if(foldouts.Count > 0)
				result.AddRange(foldouts);

			foreach(var child in element.hierarchy.Children())
			{
				var nestedFoldouts = GetAllChildVisualElementsRecursive(child);
				if(nestedFoldouts.Count > 0)
					result.AddRange(nestedFoldouts);
			}

			return result;
		}

		public static T GetFirstVisualElementInSelfOrChild<T>(VisualElement element) where T : VisualElement
		{
			var inSelf = element.Q<T>();
			if(inSelf != null)
			{
				return inSelf;
			}

			var children = GetAllChildVisualElementsRecursive(element);
			return children.FirstOrDefault(x => x is T) as T;
		}

		public static void ExpandPropertyRecursive(SerializedProperty property, bool isExpanded)
		{
			property.isExpanded = isExpanded;

			if(property.hasVisibleChildren)
			{
				foreach(var child in property.GetVisibleChildren())
				{
					ExpandPropertyRecursive(child, isExpanded);
				}
			}
		}

		public static VisualElement CreateBackgroundArea()
		{
			ColorUtility.TryParseHtmlString(EditorGUIUtility.isProSkin ? "#242424" : "#7F7F7F", out var borderColor);
			ColorUtility.TryParseHtmlString(EditorGUIUtility.isProSkin ? "#414141" : "#C1C1C1", out var backGroundColor);
			var background = new VisualElement
			{
				name = "PropertyContainer",
				style =
				{
					borderLeftColor = borderColor,
					borderRightColor = borderColor,
					borderTopColor = borderColor,
					borderBottomColor = borderColor,
					borderTopLeftRadius = 3,
					borderBottomLeftRadius = 3,
					borderTopRightRadius = 3,
					borderBottomRightRadius = 3,
					borderLeftWidth = 1,
					borderRightWidth = 1,
					borderTopWidth = 1,
					borderBottomWidth = 1,
					backgroundColor = backGroundColor
				}
			};

			return background;
		}
		
		#endregion

		#region Private
		private static string Serialize(object source)
		{
			return JsonConvert.SerializeObject(source, Formatting.Indented, JsonNetUtility.defaultSettings);
		}

		private static T Deserialize<T>(string jsonText)
		{
			return JsonConvert.DeserializeObject<T>(jsonText, JsonNetUtility.defaultSettings);
		}
		#endregion
    }
}

