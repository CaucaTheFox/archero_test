using System.Collections.Generic;

namespace Core.ResourceManagement
{
	public interface IResourceManager
	{
		T LoadResource<T>(string resourceName) where T: UnityEngine.Object;
		IList<T> LoadResources<T>(string path) where T: UnityEngine.Object;
        T LoadAssetUri<T>(string guidAndPath) where T : UnityEngine.Object;

    }

	static class ResourceManagerExtension
	{
		public static T LoadAssetUriIfNotNull<T>(this IResourceManager resourceManager, string guidAndPath)
			where T : UnityEngine.Object
		{
			return !string.IsNullOrEmpty(guidAndPath) 
				? resourceManager.LoadAssetUri<T>(guidAndPath) 
				: null;
		}
	}
}
