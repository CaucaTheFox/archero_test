using Core.IoC;
using Core.ResourceManagement;
using Newtonsoft.Json;
using UnityEngine;
using WanzyeeStudio.Json;

public interface IJsonConfig<out A>
{
    A Value { get; }
}

public abstract class JsonConfig<A> : IJsonConfig<A> where A : class
{
    [Inject] protected IResourceManager resourceManager;

    protected abstract string ConfigPath { get; }

    private A config;

    public A Value => config ?? (config = LoadConfig());

    private A LoadConfig()
    {
        var resource = resourceManager.LoadResource<TextAsset>(ConfigPath);
        var cfg = JsonConvert.DeserializeObject<A>(resource.text, JsonNetUtility.defaultSettings);
        return cfg;
    }
}