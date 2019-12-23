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
        }

        private class EnemyConfigContainer : JsonConfig<EnemyConfig>
        {
            protected override string ConfigPath => "Configs/Enemy/EnemyConfig";
        }
    }
}
