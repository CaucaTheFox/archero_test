using System.Collections.Generic;
using System.Linq;
using Features.Enemies;
using Tools;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utility
{
    public class EnemyBehaviourCatalogueConfigEditor : EditorWindow
    {
        #region Constants
        private const string DirectoryPath = "Assets/Resources/Configs/Enemies/";
        private const string ConfigName = "EnemyBehaviourCatalogueConfig.json";
        #endregion

        #region Unity Serialized Fields
        [SerializeField] private EnemyBehaviourCatalogueConfig enemyBehaviourCatalogueConfig;
        #endregion
        
        #region State
        private SerializedObject serializedObject;
        private SerializedProperty serializedConfig;
        private List<SerializedProperty> serializedProperties;
        
        private List<string> idDropdownOptions;
        
        private Button saveButton;
        private Button addConfigButton;
        private PropertyField scrollViewPropertyField;
        private PropertyField configPropertyField;
        private VisualElement idSelectionContainer;
        private DropdownField idDropdownField;
        private ScrollView configScrollView;
        #endregion

        #region Lifecycle
        [MenuItem("ArcheroTest/Editors/Enemy Behaviour Catalogue Config Editor")]
        private static void Init()
        {
            var window = (EnemyBehaviourCatalogueConfigEditor)GetWindow(typeof(EnemyBehaviourCatalogueConfigEditor));
            window.Show();
        }

        private void InitWindow()
        {
            enemyBehaviourCatalogueConfig ??=
                EditorScriptUtility.LoadFileFromDirectory<EnemyBehaviourCatalogueConfig>(DirectoryPath, ConfigName) ??
                CreateEnemyBehaviourCatalogueConfig();
            
            UpdateSerializedObject();
        }
        
        private void CreateGUI()
        {
            InitWindow();
            
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Features/Enemies/Configs/Editor/EnemyBehaviourConfigEditor.uxml");
            var visualTreeInstance = visualTree.Instantiate();
            rootVisualElement.Add(visualTreeInstance);
            
            saveButton = rootVisualElement.Q<Button>("SaveButton");
            saveButton.clicked += HandleSaveButtonClicked;
            
            addConfigButton = rootVisualElement.Q<Button>("AddConfigButton");
            addConfigButton.clicked += HandleAddConfigButtonClicked;
            
            UpdateDropdownOptions();
            
            idSelectionContainer = rootVisualElement.Q<VisualElement>("IdSelectionContainer");
            idDropdownField = idSelectionContainer.Q<DropdownField>("IdDropdown");
            EditorScriptUtility.InitIdSelectionContainer(idSelectionContainer, idDropdownOptions,
                HandleSelectedIdChanged, UpdateIdSelectionButtonLabels, HandlePreviousIdButtonClicked, HandleNexIdButtonClicked);
            
            scrollViewPropertyField = rootVisualElement.Q<PropertyField>("ScrollViewPropertyField");
         
            configPropertyField = rootVisualElement.Q<PropertyField>("ConfigPropertyField");
            configPropertyField.BindProperty(serializedProperties[0]);
            EditorScriptUtility.GetFirstVisualElementInSelfOrChild<Toggle>(configPropertyField).value = true;

            configScrollView = rootVisualElement.Q<ScrollView>("ConfigScrollView");
            configScrollView.style.display = DisplayStyle.None;
        }
        #endregion

        #region Private
        private void UpdateSerializedObject()
        {
            serializedObject = new SerializedObject(this);
            serializedConfig = serializedObject.FindProperty("enemyBehaviourCatalogueConfig");
            
            var serializedConfigList = serializedConfig.FindPropertyRelative("configList"); 
            serializedProperties = serializedConfigList.GetVisibleChildren().ToList();
            serializedProperties.RemoveAt(0);
        }
        
        private void SaveConfig(EnemyBehaviourCatalogueConfig catalogueConfig)
        {
            EditorScriptUtility.SaveConfig(catalogueConfig,DirectoryPath + ConfigName);
        }

        private void AddEnemyBehaviourConfig()
        {
            var stringId = CreateEnemyBehaviourConfigId(enemyBehaviourCatalogueConfig.Configs.Count);
            var enemyBehaviourConfig = CreateEnemyBehaviourConfig(stringId);
            enemyBehaviourCatalogueConfig.AddEnemyBehaviourConfig(enemyBehaviourConfig);
        }

        private EnemyBehaviourCatalogueConfig CreateEnemyBehaviourCatalogueConfig()
        {
            var enemyBehaviourConfigs = new List<EnemyBehaviourConfig>();
            var stringId = CreateEnemyBehaviourConfigId(0);
            var enemyBehaviourConfig = CreateEnemyBehaviourConfig(stringId);
            enemyBehaviourConfigs.Add(enemyBehaviourConfig);
            
            var config = new EnemyBehaviourCatalogueConfig(enemyBehaviourConfigs);
            SaveConfig(config);
            AssetDatabase.Refresh();
            return config;
        }

        private string CreateEnemyBehaviourConfigId(int count)
        {
            var numericId = EditorScriptUtility.GetNumericIdTwoDigits(count);
            return "EnemyBehaviourConfig_" + numericId;
        }

        private EnemyBehaviourConfig CreateEnemyBehaviourConfig(string id)
        {
            var config = new EnemyBehaviourConfig()
            {
                Id = id,
                EnemyBehaviourActionData = new List<EnemyBehaviourActionData>()
            };
            return config;
        }
        
        private (string, string) UpdateIdSelectionButtonLabels(string selectedDropdownOption)
        {
            var selectedTrainingCampIndex = idDropdownOptions.IndexOf(selectedDropdownOption);
            var nextIndex = selectedTrainingCampIndex + 1;

            if(nextIndex >= idDropdownOptions.Count)
            {
                nextIndex = 0;
            }

            var previousIndex = selectedTrainingCampIndex - 1;

            if(previousIndex < 0)
            {
                previousIndex = idDropdownOptions.Count - 1;
            }

            var previousId = idDropdownOptions[previousIndex].Split(' ')[0];
            var nextId = idDropdownOptions[nextIndex].Split(' ')[0];

            return ($"Prev: {previousId}", $"Next: {nextId}");
        }

        private void UpdateDropdownOptions()
        {
            idDropdownOptions = serializedProperties
                .Select(x => x.FindPropertyRelative("Id").stringValue)
                .ToList(); 
        }
        
        private void HandleSaveButtonClicked()
        {
            serializedObject.ApplyModifiedProperties();
            SaveConfig(enemyBehaviourCatalogueConfig);
            InitWindow();
        }

        private void HandleAddConfigButtonClicked()
        {
            AddEnemyBehaviourConfig();
            SaveConfig(enemyBehaviourCatalogueConfig);
            InitWindow();
            UpdateDropdownOptions();
            idDropdownField.choices = idDropdownOptions;
            idDropdownField.index = idDropdownField.choices.Count - 1;
            UpdateIdSelectionButtonLabels(idDropdownOptions[idDropdownField.index]);
        }

        private void HandleSelectedIdChanged(ChangeEvent<string> evt, DropdownField dropdownField)
        {
            var propertyField = configPropertyField;
            
            propertyField.Unbind();
            propertyField.BindProperty(serializedProperties[idDropdownField.index]);
            EditorScriptUtility.GetFirstVisualElementInSelfOrChild<Toggle>(propertyField).value = true;
        }

        private void HandlePreviousIdButtonClicked(DropdownField dropdownField)
        {
            var previousIndex = dropdownField.index - 1;
            if(previousIndex < 0)
                previousIndex = dropdownField.choices.Count - 1;

            dropdownField.index = previousIndex;
        }

        private void HandleNexIdButtonClicked(DropdownField dropdownField)
        {
            var nextIndex = dropdownField.index + 1;
            if(nextIndex >= dropdownField.choices.Count)
                nextIndex = 0;

            dropdownField.index = nextIndex;
        }
        #endregion
    }
}