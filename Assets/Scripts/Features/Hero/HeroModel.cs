using Core.IoC;
using UnityEngine;

namespace Features.Heroes
{
    public interface IHeroModel
    {
        Hero HeroInstance { get; set; }
        Vector3 HeroPosition { get; }
        HeroSettings GetHeroBaseSettings(string id);

    }

    public class HeroModel : IHeroModel
    {
        #region Dependencies
        [Inject] private IJsonConfig<HeroConfig> heroConfig;
        #endregion

        #region Properties
        public Hero HeroInstance { get; set; }
        public Vector3 HeroPosition => HeroInstance.Position; 
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
