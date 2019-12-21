using Core;
using Core.IoC;
using Core.ResourceManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Screens
{
    public interface IScreenManager
    {
        event Action<IScreenController> OnScreenChanged;
        IScreenController CurrentScreen { get; }
        
        void RegisterFactory(string prefabPath, IScreenFactory factory);
        
        T Push<T>(string prefabPath) where T: class, IScreenController;
        T Replace<T>(string prefabPath) where T: class, IScreenController;
        void Pop();
    }

    public interface IScreenFactory
    {
        IScreenController Create();
    }

    public interface IScreenController
    {
        bool Visible { get; set; }
        string Name { get; }
        void DestroyScreen();
    }
    
    public class ScreenManager: IScreenManager
    {
        #region - Constants
        private const string LogTag = "ScreenManager";
        #endregion
        
        #region - Events
        public event Action<IScreenController> OnScreenChanged;
        #endregion
        
        #region - Properties

        public IScreenController CurrentScreen
        {
            get => currentScreen;
            private set {
                currentScreen = value;
                OnScreenChanged?.Invoke(currentScreen);
            }
        }
        #endregion
        
        #region - Dependencies
#pragma warning disable 649
        [Inject] private ISceneLayers layers;
        [Inject] private IResourceManager resourceManager;
#pragma warning restore 649

        #endregion
        
        #region - State
        private readonly Stack<IScreenController> screensStack = new Stack<IScreenController>();
        private readonly Dictionary<string, IScreenFactory> factories = new Dictionary<string, IScreenFactory>();
        private IScreenController currentScreen;
        #endregion
        
        #region - Public
        public void RegisterFactory(string prefabPath, IScreenFactory factory)
        {
            factories.Add(prefabPath, factory);
        }
        
        public T Push<T>(string prefabPath) where T: class, IScreenController
        {
            var screen = LoadAndInstantiate<T>(prefabPath);

            if (screensStack.Count > 0) {
                var current = screensStack.Peek();
                current.Visible = false;
            }
            
            screensStack.Push(screen);

            CurrentScreen = screen;

            return screen;
        }

        public void Pop()
        {
            if (screensStack.Count < 2) {
                return;
            }

            var current = screensStack.Pop();
            current.DestroyScreen();

            current = screensStack.Peek();
            current.Visible = true;
            
            CurrentScreen = current;
            
            Resources.UnloadUnusedAssets();
        }
        
        public T Replace<T>(string prefabPath) where T: class, IScreenController
        {
            if (screensStack.Count > 0) {
                var current = screensStack.Pop();
                current.DestroyScreen();
            }
            
            var screen = LoadAndInstantiate<T>(prefabPath);

            screensStack.Push(screen);
            
            CurrentScreen = screen;

            Resources.UnloadUnusedAssets();

            return screen;
        }
        #endregion
        
        #region - Private
        private T LoadAndInstantiate<T>(string prefabPath) where T: class, IScreenController
        {
            IScreenFactory factory;

            if (factories.TryGetValue(prefabPath, out factory)) {
                return (T)factory.Create();
            }
            
            var prefab = resourceManager.LoadResource<GameObject>(prefabPath);
            if (prefab == null) {
                return null;
            }

            if (prefab.GetComponent<T>() == null) {
                throw new Exception(string.Format("[{0}] Prefab {1} doesn't have the component {2}",
                    LogTag,
                    prefab.name,
                    typeof(T).Name
                ));
            }

            var parent = layers.GetLayerTransform(SceneLayer.ScreenLayer);
            var gameObject = GameObject.Instantiate(prefab, parent);

            if (gameObject == null) {
                return null;
            }

            gameObject.name = prefab.name;

            return gameObject.GetComponent<T>();
        }
        #endregion
    }

    public static class ScreenManagerExtension
    {
        public static void RegisterScreen(this IScreenManager screenManager, Type controllerType, string path2D, string path3D)
        {
            var baseType = controllerType.BaseType;
            
            if (baseType == null) {
                throw new Exception(string.Format(
                    "Type {0} should be derived from DualScreenController",
                    controllerType.Name
                ));
            }
            
            var viewTypes = baseType.GetGenericArguments();
            var screenNameField = controllerType.GetField("ScreenName");

            var screenName = screenNameField != null ?
                (string) screenNameField.GetValue(null) :
                controllerType.Name;
            
            var factoryGenericType = typeof(DualScreenFactory<,,>);
            var genericParameters = new []{ controllerType, viewTypes[0], viewTypes[1] };
            var factoryType = factoryGenericType.MakeGenericType(genericParameters);

            var factory = Activator.CreateInstance(factoryType, path2D, path3D);
            
            GameContext.Container.ResolveAll(factory);
            
            screenManager.RegisterFactory(screenName, factory as IScreenFactory);
        }
        
        public static void RegisterScreen(this IScreenManager screenManager, Type controllerType, string viewPrefabPath)
        {
            var baseType = controllerType.BaseType;
            
            if (baseType == null) {
                throw new Exception($"Type {controllerType.Name} should be derived from DualScreenController");
            }
            
            var viewTypes = baseType.GetGenericArguments();
            var screenName = GetScreenName(controllerType);
            
            var factoryGenericType = typeof(ScreenFactory<,>);
            var genericParameters = new []{ controllerType, viewTypes[0] };
            var factoryType = factoryGenericType.MakeGenericType(genericParameters);

            var factory = Activator.CreateInstance(factoryType, viewPrefabPath);
            
            GameContext.Container.ResolveAll(factory);
            
            screenManager.RegisterFactory(screenName, factory as IScreenFactory);
        }

        public static ScreenControllerType Push<ScreenControllerType>(this IScreenManager screenManager)
            where ScreenControllerType : class, IScreenController
        {
            var screenName = GetScreenName(typeof(ScreenControllerType));
            return screenManager.Push<ScreenControllerType>(screenName);
        }
        
        public static ScreenControllerType Replace<ScreenControllerType>(this IScreenManager screenManager)
            where ScreenControllerType : class, IScreenController
        {
            var screenName = GetScreenName(typeof(ScreenControllerType));
            return screenManager.Replace<ScreenControllerType>(screenName);
        }

        private static string GetScreenName(Type controllerType)
        {
            var screenNameField = controllerType.GetField("ScreenName");

            return screenNameField != null ?
                (string) screenNameField.GetValue(null) :
                controllerType.Name;
        }
    }
}