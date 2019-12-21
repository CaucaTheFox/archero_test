using Core.IoC;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Features.Screens
{
    public interface IScreenExtension
    {
        void Exit();
    }

    public interface IScreenExtension<ScreenType>: IScreenExtension
    {
        void Enter(ScreenType screen);
    }
    
    public interface IScreenExtensions
    {
        void RegisterScreenExtension(IScreenExtension extension);
    }
    
    public class ScreenExtensionWrapper
    {
        public readonly IScreenExtension extension;
        public readonly Type ScreenType;
        
        private readonly MethodInfo enterMethod; 

        public ScreenExtensionWrapper(IScreenExtension extension)
        {
            this.extension = extension;
            var extensionType = extension.GetType();
            var interfaces = extensionType.GetInterfaces();

            foreach (var extensionInterface in interfaces) {
                if (!extensionInterface.IsGenericType) {
                    continue;
                }

                if (extensionInterface.GetGenericTypeDefinition() != typeof(IScreenExtension<>)) {
                    continue;
                }

                ScreenType = extensionInterface.GetGenericArguments()[0];
                enterMethod = extensionInterface.GetMethod("Enter");
                break;
            }
        }

        public void Enter(IScreenController screenController)
        {
            enterMethod.Invoke(extension, new object[] {screenController});
        }

        public void Exit()
        {
            extension.Exit();
        }
    }
    
    public class ScreenExtensions: IScreenExtensions
    {
        #region - Dependencies
        [Inject] private IScreenManager screenManager;
        #endregion
        
        #region - State
        private Type currentScreenType;
        private Dictionary<Type, List<ScreenExtensionWrapper>> screenExtensions = new Dictionary<Type, List<ScreenExtensionWrapper>>();
        #endregion

        #region - Lifecycle
        private void Init()
        {
            screenManager.OnScreenChanged += OnScreenChangedHandler;
        }
        #endregion
        
        #region - Public
        public void RegisterScreenExtension(IScreenExtension extension)
        {
            var extensionWrapper = new ScreenExtensionWrapper(extension);
            
            List<ScreenExtensionWrapper> extensionsList;
            if (!screenExtensions.TryGetValue(extensionWrapper.ScreenType, out extensionsList)) {
                extensionsList = new List<ScreenExtensionWrapper>();
                screenExtensions.Add(extensionWrapper.ScreenType, extensionsList);
            }
            
            extensionsList.Add(extensionWrapper);
        }
        #endregion
        
        #region - Private
        private void OnScreenChangedHandler(IScreenController screenController)
        {
            if (currentScreenType != null) {
                NotifyAboutExit(currentScreenType);
            }

            currentScreenType = GetScreenType(screenController);
            
            NotifyAboutEnter(currentScreenType, screenController);
        }

        private void NotifyAboutEnter(Type screenType, IScreenController controller)
        {
            ForAllExtensionsForScreen(screenType, extension => {
                extension.Enter(controller);
            });
        }

        private void NotifyAboutExit(Type screenType)
        {
            ForAllExtensionsForScreen(screenType, extension => {
                extension.Exit();
            });
        }

        private void ForAllExtensionsForScreen(Type screenType, Action<ScreenExtensionWrapper> action)
        {
            if (!screenExtensions.TryGetValue(screenType, out var extensionsList)) {
                return;
            }

            foreach (var extension in extensionsList) {
                action(extension);
            }
        }

        private Type GetScreenType(IScreenController controller)
        {
            return controller.GetType();
        }
        #endregion
    }
}