using Core.IoC;

namespace Core.RenderFeatures
{
    public class RenderFeaturesServiceRegistration: IServiceRegistration
    {
        public void RegisterServices(IIoC container)
        {
            container.RegisterSingleton<IRenderFeatureModel, RenderFeatureModel>();
        }
    }
}