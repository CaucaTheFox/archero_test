using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "ModuleConfig", menuName = "Configs/General/Module Config")]
    public class ModuleConfig : ScriptableObject
    {
        public List<string> modules;
    }
}