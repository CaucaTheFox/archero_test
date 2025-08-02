using Core;
using Core.IoC;

namespace Features.Enemies
{

    public class EnemyFeature : IFeatureInitialization, IServiceRegistration
    {
        public void Init()
        {

        }

        public void RegisterServices(IIoC container)
        {
            container.RegisterSingleton<IEnemiesModel, EnemiesModel>();
            
            container.RegisterSingleton<IJsonConfig<EnemyConfig>, EnemyConfigContainer>();
            container.RegisterSingleton<IJsonConfig<EnemyBehaviourCatalogueConfig>, EnemyBehaviourCatalogueConfigContainer>();
        }

        private class EnemyConfigContainer : JsonConfig<EnemyConfig>
        {
            protected override string ConfigPath => "Configs/Enemies/EnemyConfig";
        }
        
        private class EnemyBehaviourCatalogueConfigContainer : JsonConfig<EnemyBehaviourCatalogueConfig>
        {
            protected override string ConfigPath => "Configs/Enemies/EnemyBehaviourCatalogueConfig";
        }
    }
}
