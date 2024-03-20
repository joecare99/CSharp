using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace BlazorWasmDocker.Models;

	public class IoC
	{
		public static IServiceProvider ServiceProvider { get; set; }
		public static T GetService<T>() => (T)ServiceProvider.GetService(typeof(T));
		public static T GetRequiredService<T>() => (T)ServiceProvider.GetRequiredService(typeof(T));
		public static object GetService(Type serviceType) => ServiceProvider.GetService(serviceType);
		public static object GetRequiredService(Type serviceType) => ServiceProvider.GetRequiredService(serviceType);
		public static IEnumerable<T> GetServices<T>() => ServiceProvider.GetServices<T>();
		public static IEnumerable<object> GetServices(Type serviceType) => ServiceProvider.GetServices(serviceType);
		public static IServiceProvider CreateScope() => ServiceProvider.CreateScope().ServiceProvider;
		public static IServiceScope CreateScope(out IServiceScope scope)
		{
			scope = ServiceProvider.CreateScope();
			return scope;
		}
	}
