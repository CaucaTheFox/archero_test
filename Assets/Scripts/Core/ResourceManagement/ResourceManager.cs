using System.Collections.Generic;
using System;
using Core.Utility;

namespace Core.ResourceManagement
{	
	public class ResourceManager: IResourceManager
	{
		#region - Constants
		private const string LogTag = "ResourceManager";
        private const char Seperator = '|';
        #endregion

        #region - State
        private readonly SortedList<int, IResourceProvider> providers = new SortedList<int, IResourceProvider>(new DuplicateKeyComparer<int>());
		#endregion

		#region - Types
		private class DuplicateKeyComparer<TKey>: IComparer<TKey> where TKey: IComparable
		{			
			public int Compare(TKey x, TKey y)
			{
				var result = x.CompareTo(y);
				return result == 0 ? 1 : result;
			}	
		}
		#endregion

		#region - Lifecycle
		private void Init()
		{
			ReflectionUtility.ForAllInstances<IResourceProvider>(RegisterProvider);
		}
		#endregion

		#region - Public
		public T LoadResource<T>(string resourceName) where T: UnityEngine.Object
		{
			T resource = null;

			foreach (var provider in providers) {
				resource = provider.Value.LoadResource<T>(resourceName);
				if (resource != null) {
					break;
				}
			}

			return resource;
		}

        public T LoadAssetUri<T>(string guidAndPath) where T : UnityEngine.Object
        {
            T resource = null;
            var path = ResolvePath(guidAndPath); 
            foreach (var provider in providers)
            {
                resource = provider.Value.LoadResource<T>(path);
                if (resource != null)
                {
                    break;
                }
            }

            return resource;
        }

        public IList<T> LoadResources<T>(string path) where T: UnityEngine.Object
		{
			IList<T> resources = null;

			foreach (var provider in providers) {
				resources = provider.Value.LoadResources<T>(path);
				if (resources != null) {
					break;
				}
			}

			return resources;
		}
		#endregion

		#region - Private
		private void RegisterProvider(IResourceProvider provider)
		{
			providers.Add(provider.Priority, provider);
		}

        private string ResolvePath(string property)
        {
            var seperatorIndex = Array.IndexOf(property.ToCharArray(), Seperator);
            return property.Substring(seperatorIndex + 1);
        }
        #endregion
    }
}
