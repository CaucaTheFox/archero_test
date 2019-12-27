using Configs;
using Core.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Core
{
    public class GameContext
    {
        private static IoC.IoC container;
        public static IoC.IoC Container {
            get => container ?? (container = CreateContainer());
            set => container = value;
        }

        public static void Init()
        {
            var container = Container;

            ReflectionUtility.ForAllInstances<IFeatureInitialization>(feature => {
                container.ResolveAll(feature);
                feature.Init();
            });
        }

        private static IoC.IoC CreateContainer ()
        {
            var container = new IoC.IoC ();

            ReflectionUtility.ForAllInstances<IServiceRegistration> (
                initializer => initializer.RegisterServices(container)
            );

            return container;
        }
    }
}
