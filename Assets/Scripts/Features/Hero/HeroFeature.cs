using Core;
using Core.IoC;

namespace Features.Heroes
{

    public class HeroFeature : IFeatureInitialization, IServiceRegistration
    {
        [Inject] private IHeroModel heroModel;

        public void Init()
        {

        }

        public void RegisterServices(IIoC container)
        {
            container.RegisterSingleton<IHeroModel, HeroModel>();
            container.RegisterSingleton<IJsonConfig<HeroConfig>, HeroConfigContainer>();
        }

        private class HeroConfigContainer : JsonConfig<HeroConfig>
        {
            protected override string ConfigPath => "Configs/Hero/HeroConfig";
        }
    }
}
