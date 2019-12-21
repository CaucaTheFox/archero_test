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

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            ReflectionUtility.ForAllInstances<IFeatureInitialization>(feature => {
                container.ResolveAll(feature);
                feature.Init();
            });

            Debug.Log($"[GameContext] Features initialized in {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Restart();

            var modulesConfig = container.Resolve<IScriptableObjectConfig<ModuleConfig>>();
            var activeModules = new HashSet<string>(modulesConfig.Value.modules);
            
            ReflectionUtility.ForAllInstances<IModule>(module => {
                if (!activeModules.Contains(module.GetType().Name)) {
                    return;
                }
                
                container.ResolveAll(module);
                module.Init();
            });

            Debug.Log($"[GameContext] Modules initialized in {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Stop();
        }

        private static IoC.IoC CreateContainer ()
        {
            var container = new IoC.IoC ();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            ReflectionUtility.ForAllInstances<IServiceRegistration> (
                initializer => initializer.RegisterServices(container)
            );

            stopwatch.Stop();

            Debug.Log($"[GameContext] Services registered in {stopwatch.ElapsedMilliseconds} ms");

            return container;
        }
    }
}
