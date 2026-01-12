using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace Core.Utility
{
	public static class ReflectionUtility
	{
		public static Type[] GetAllTypes (Assembly assembly = null)
		{
			var currentAssembly = assembly ?? typeof(ReflectionUtility).Assembly;
			return currentAssembly.GetTypes ();
		}

		public static List<T> InstantiateAll<T> (Assembly assembly = null) where T : class
		{
			var result = new List<T> ();
			var baseType = typeof (T);

			foreach (var type in GetAllTypes (assembly)) {
				if (!type.IsClass || !(baseType.IsAssignableFrom (type))) {
					continue;
				}

				var instance = Activator.CreateInstance(type) as T;
				result.Add (instance);
			}

			return result;
		}

		public static void ForAllInstances<T> (Action<T> callback, Assembly assembly = null) where T : class
		{
			var baseType = typeof (T);
			
			foreach (var type in GetAllTypes (assembly)) {
				if (!type.IsClass || !(baseType.IsAssignableFrom (type)) || type.IsGenericType) {
					continue;
				}
				
				var instance = Activator.CreateInstance(type) as T;
				callback (instance);
			}
		}

		public static IEnumerable<Type> GetAllTypesOf<T>() where T : class =>
			GetAllTypes().Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass);

		public static Attribute GetAttribute<T> (MethodInfo method)
		{
			var attributes = method.GetCustomAttributes (true);
			var count = attributes.Length;
			
			for (var i = 0; i < count; ++i) {
				var attribute = attributes[i];

				if (attribute.GetType () == typeof (T)) {
					return attribute as Attribute;
				}
			}

			return null;
		}

		public static bool HasAttribute(this FieldInfo field, Type attributeType)
		{
			var attributes = field.GetCustomAttributes(inherit: true);
			var count = attributes.Length;
			for (var i = 0; i < count; i += 1) {
				var attribute = attributes[i];
				if (attribute.GetType() == attributeType) {
					return true;
				}
			}

			return false;
		}
		
		public static bool HasAttribute(this MethodInfo method, Type attributeType)
		{
			var attributes = method.GetCustomAttributes(inherit: true);
			var count = attributes.Length;
			for (var i = 0; i < count; i += 1) {
				var attribute = attributes[i];
				if (attribute.GetType() == attributeType) {
					return true;
				}
			}

			return false;
		}

		public static void ExecuteAllStaticWithAttribute<T>(Assembly assembly = null) where T: Attribute
		{
			foreach (var type in GetAllTypes(assembly)) {
				var methods = type.GetMethods(
					BindingFlags.Public |
					BindingFlags.Static |
					BindingFlags.NonPublic
				);
				
				foreach (var method in methods) {
					if (GetAttribute<T>(method) == null) {
						continue;
					}
					method.Invoke(null, null);
				}
			}
		}
	}
}
