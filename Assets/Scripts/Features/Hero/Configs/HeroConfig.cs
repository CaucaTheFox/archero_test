using Newtonsoft.Json;
using System.Collections.Generic;

namespace Features.Heroes
{
    public class HeroConfig
    {
        public Dictionary<string, HeroSettings> Heroes;

        [JsonConstructor]
        public HeroConfig(Dictionary<string, HeroSettings> heroes)
        {
            Heroes = heroes;
        }

        public HeroConfig(List<HeroSettings> heroes)
        {
            Heroes = new Dictionary<string, HeroSettings>();
            foreach (var hero in heroes)
            {
                Heroes.Add(hero.Id, hero);
            }
        }
    }

    public class HeroSettings
    {
        public string Id;
        public string Name;
        public int Health;
        public int Attack;
        public float AttackSpeed;
        public int DamageResistance;
        public float Dodge;
    }
}
