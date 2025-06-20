using Core.Utility;

namespace Core
{
    public class GameContext
    {
        #region Properties
        public static IoC.IoC Container => container ??= CreateContainer();
        #endregion
        
        #region State
        private static IoC.IoC container;
        #endregion
        
        #region Lifecycle
        public static void Init()
        {
            ReflectionUtility.ForAllInstances<IFeatureInitialization>(feature => {
                Container.ResolveAll(feature);
                feature.Init();
            });
        }
        #endregion

        #region Private
        private static IoC.IoC CreateContainer()
        {
            var container = new IoC.IoC ();

            ReflectionUtility.ForAllInstances<IServiceRegistration> (
                initializer => initializer.RegisterServices(container)
            );

            return container;
        }
        #endregion
    }
}
