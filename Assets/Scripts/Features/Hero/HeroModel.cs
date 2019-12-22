using Core.IoC;
using Core.ResourceManagement;

namespace Features.Heroes
{
    public interface IHeroModel
    {
        HeroSettings GetHeroBaseSettings(string id);
    }

    public class HeroModel : IHeroModel
    {
        #region Dependencies
        [Inject] private IJsonConfig<HeroConfig> heroConfig;
        #endregion

        #region Public
        public HeroSettings GetHeroBaseSettings(string id)
        {
            heroConfig.Value.Heroes.TryGetValue(id, out var heroSettings);
            return heroSettings;
        }
        #endregion
    }
}
