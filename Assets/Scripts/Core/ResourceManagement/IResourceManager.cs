using System.Collections.Generic;

namespace Core.ResourceManagement
{
	public interface IResourceManager
	{
		T LoadResource<T>(string resourceName) where T: UnityEngine.Object;
		IList<T> LoadResources<T>(string path) where T: UnityEngine.Object;
        T LoadAssetUri<T>(string guidAndPath) where T : UnityEngine.Object;

    }
}
