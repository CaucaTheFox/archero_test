using System.Collections.Generic;
using UnityEngine;

namespace Core.ResourceManagement
{
	public class ProviderFromResources: IResourceProvider
	{
		const int PriorityValue = 1;

		public int Priority => PriorityValue;

		public T LoadResource<T>(string resourceName) where T: Object
		{
			return Resources.Load<T>(resourceName);
		}

		public IList<T> LoadResources<T>(string path) where T : Object
		{
			return Resources.LoadAll<T>(path);
		}
	}
}
