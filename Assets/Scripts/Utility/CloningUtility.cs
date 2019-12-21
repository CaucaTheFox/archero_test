using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public interface ICloneable<out A>
    {
        A Clone();
    }

    public static class CloningUtility
    {
        public static Dictionary<A, B> Clone<A, B>(this Dictionary<A, B> dict) where B : class, ICloneable<B> =>
            dict?
                .Select(pair => new KeyValuePair<A, B>(pair.Key, pair.Value?.Clone()))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

        public static Dictionary<A, B> CloneShallow<A, B>(this Dictionary<A, B> dict) =>
            dict == null ? null : new Dictionary<A, B>(dict);

        public static List<A> Clone<A>(this List<A> list) where A : class, ICloneable<A> =>
            list?
                .Select(value => value?.Clone())
                .ToList();

        public static List<A> CloneShallow<A>(this List<A> list) =>
            list == null ? null : new List<A>(list);

        public static HashSet<A> Clone<A>(this HashSet<A> set) where A : class, ICloneable<A> =>
            set == null 
                ? null 
                : new HashSet<A>(set.Select(value => value?.Clone()));

        public static HashSet<A> CloneShallow<A>(this HashSet<A> set) =>
            set == null 
                ? null 
                : new HashSet<A>(set);
    }
}