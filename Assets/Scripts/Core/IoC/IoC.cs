using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Utility;

namespace Core.IoC
{
    public interface IClassFactory
	{
		object Create ();
	}

	public interface IClassFactory<T> : IClassFactory where T : class
	{
		//T create ();
	}

	public interface ISmartFactory
	{
		IIoC Container { get; set; }
	}

	public interface ISmartFactory<T>
		: IClassFactory<T>
			, ISmartFactory
		where T : class
	{
	}
	
	public interface IIoC
	{
		void Register<Interface, ClassType>() where Interface: class where ClassType: class, Interface;
		void RegisterSingleton<Interface, ClassType>() where Interface: class where ClassType: class, Interface;

		Interface Resolve<Interface>() where Interface: class;
		void ResolveAll(object target);
		object Resolve(Type type);
		object GetServiceSilent(Type type);
	}
	
	public class IoC: IIoC
	{
		const string LogTag = "IoC";

		class ServiceFactory
		{
			public IClassFactory factory;
			public bool isSingleton;

			public ServiceFactory (IClassFactory factory, bool isSingleton)
			{
				this.factory = factory;
				this.isSingleton = isSingleton;
			}
		}

		Dictionary<Type, ServiceFactory> factories = new Dictionary<Type, ServiceFactory> ();
		Dictionary<Type, object> instances = new Dictionary<Type, object> ();

		public void Register<Interface, ClassType>() where Interface: class where ClassType: class, Interface
		{
			factories.Add(typeof(Interface), new ServiceFactory(new SmartFactory<Interface, ClassType>(this), false));
		}

		public void RegisterSingleton<Interface, ClassType>() where Interface: class where ClassType: class, Interface
		{
			factories.Add(typeof(Interface), new ServiceFactory(new SmartFactory<Interface, ClassType>(this), true));
		}
		
		public Interface Resolve<Interface>() where Interface : class
		{
			return Resolve(typeof(Interface)) as Interface;
		}

		public object Resolve(Type type)
		{
			var service = GetServiceSilent(type);

			if (service != null) {
				return service;
			}
#if IOC_LOG
			if (!factories.ContainsKey(typeof(ILog))) {
				return null;
			}
			var log = Resolve<ILog>();
			log.LogErrorFormat(LogTag, "Unresolved dependency: {0}", type.FullName);
#endif

			return null;
		}

		public void ResolveAll(object target)
		{
			var type = target.GetType();
			var fields = type.GetFields(
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy
			);
			
			foreach (var field in fields)
			{
				if (!field.HasAttribute(typeof(InjectAttribute))) {
					continue;
				}
				
				field.SetValue(target, Resolve(field.FieldType));
			}
		}

		public object GetServiceSilent(Type type)
		{
			if (type == typeof(IIoC)) {
				return this;
			}

			ServiceFactory serviceFactory;

			if (!factories.TryGetValue(type, out serviceFactory)) {
				return null;
			}

			if (serviceFactory.isSingleton) {
				if (instances.ContainsKey(type)) {
					return instances[type];
				}

				var newInstance = serviceFactory.factory.Create();
				instances.Add (type, newInstance);
				return newInstance;
			}
			return serviceFactory.factory.Create();
		}
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class InjectAttribute: Attribute
	{
	}

	public class DefaultFactory<Interface, T> : IClassFactory<Interface>
		where Interface : class
		where T : Interface, new()
	{
		public object Create ()
		{
			return new T ();
		}
	}

	public class SmartFactory<Interface, T> : ISmartFactory<Interface>
		where Interface : class
		where T : Interface
	{
		public IIoC Container { get; set; }

		private readonly List<Type> constructorInjections;
		private readonly List<FieldInfo> fieldsInjections;
		private readonly ConstructorInfo constructor;
		private readonly MethodInfo initMethod;

		public SmartFactory (IIoC container = null)
		{
			var type = typeof(T);
			var constructors = type.GetConstructors ();
			Container = container;

			if (constructors.Length == 1) {
				constructor = constructors [0];
			} else {
				//error!!!
				throw new Exception ("SmartFactory could only instantinate the class with a single constructor!");
			}

			var constructorParameters = constructor.GetParameters();

			constructorInjections = new List<Type> ();

			foreach (var parameter in constructorParameters) {
				constructorInjections.Add (parameter.ParameterType);
			}
			
			fieldsInjections = new List<FieldInfo>();

			var fields = type.GetFields(
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
			);
			
			foreach (var field in fields)
			{
				if (!field.HasAttribute(typeof(InjectAttribute))) {
					continue;
				}
				
				fieldsInjections.Add(field);
			}
			
			initMethod = type.GetMethod("Init",
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
			);
		}

		public object Create ()
		{
			var parameters = new object[constructorInjections.Count];
			var count = parameters.Length;

			for (var i = 0; i < count; ++i) {
				var service = Container.Resolve(constructorInjections[i]);

				parameters[i] = service ?? throw new Exception($"Can't resolve all dependencies for {typeof(T).Name}");
			}

			var newObject = constructor.Invoke(parameters);
			
			count = fieldsInjections.Count;

			for (var i = 0; i < count; i += 1) {
				var field = fieldsInjections[i];
				var service = Container.Resolve(field.FieldType);
				field.SetValue(newObject, service);
			}

			var factory = newObject as ISmartFactory;
			
			if (factory != null) {
				factory.Container = Container;
			}
			
			if (initMethod != null) {
				initMethod.Invoke(newObject, null);
			}

			return newObject;
		}
	}
}