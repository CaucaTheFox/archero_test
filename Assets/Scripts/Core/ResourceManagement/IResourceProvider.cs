using System.Collections.Generic;

namespace Core.ResourceManagement
{
	public interface IResourceProvider
	{
		int Priority { get; }

		T LoadResource<T>(string resourceName) where T: UnityEngine.Object;
		IList<T> LoadResources<T>(string path) where T: UnityEngine.Object;
	}
}
