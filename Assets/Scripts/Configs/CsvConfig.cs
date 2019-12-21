using Core.IoC;
using Core.ResourceManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Configs
{
    public interface ICsvConfig<A>
    {
        List<A> Value { get; }
    }

    public abstract class CsvConfig<A> : ICsvConfig<A> where A : class, new()
    {
        [Inject] protected IResourceManager resourceManager;

        protected abstract string ConfigPath { get; }

        private List<A> config;

        public List<A> Value => config ?? (config = LoadConfig());

        private List<A> LoadConfig()
        {
            var resource = resourceManager.LoadResource<TextAsset>(ConfigPath);

            if (resource == null)
            {
                throw new Exception($"Can't load {ConfigPath}");
            }

            var cfg = CsvParser.Parse<A>(resource.text, false, ',');
            return cfg;
        }
    }
}