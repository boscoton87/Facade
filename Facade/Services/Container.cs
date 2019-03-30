using System;
using System.Collections.Generic;
using System.Reflection;
using Facade.Helpers;
using Facade.Interfaces;
using Facade.Models;

namespace Facade.Services {
	public class Container : IContainer {
		private static Dictionary<Type, object> GlobalInstanceSet { get; set; } = new Dictionary<Type, object>();

		private static Dictionary<Type, ConstructorContext> GlobalTypeSet { get; set; } = new Dictionary<Type, ConstructorContext>();

		private static Dictionary<string, MethodContext> GlobalMethodSet { get; set; } = new Dictionary<string, MethodContext>();

		private Dictionary<Type, object> InstanceSet { get; set; } = new Dictionary<Type, object>();

		private Dictionary<Type, ConstructorContext> TypeSet { get; set; } = new Dictionary<Type, ConstructorContext>();

		private Dictionary<string, MethodContext> MethodSet { get; set; } = new Dictionary<string, MethodContext>();

		public static void RegisterGlobalMethod( string methodKey, MethodInfo methodInfo, object methodOwner ) {
			ContainerHelpers.RegisterMethod( methodKey, methodInfo, methodOwner, GlobalMethodSet );
		}

		public void RegisterMethod( string methodKey, MethodInfo methodInfo, object methodOwner ) {
			ContainerHelpers.RegisterMethod( methodKey, methodInfo, methodOwner, MethodSet );
		}

		public static void RegisterGlobalInstance<Interface>( object instance ) {
			ContainerHelpers.RegisterInstance<Interface>( instance, GlobalInstanceSet );
		}

		public void RegisterInstance<Interface>( object instance ) {
			ContainerHelpers.RegisterInstance<Interface>( instance, InstanceSet );
		}

		public static void RegisterGlobalType<Interface, RegisteredType>() {
			ContainerHelpers.RegisterType<Interface, RegisteredType>( new List<KeyValuePair<Type, object>>(), GlobalTypeSet );
		}

		public void RegisterType<Interface, RegisteredType>() {
			ContainerHelpers.RegisterType<Interface, RegisteredType>( new List<KeyValuePair<Type, object>>(), TypeSet );
		}

		public static void RegisterGlobalType<Interface, RegisteredType>( IList<KeyValuePair<Type, object>> parameters ) {
			ContainerHelpers.RegisterType<Interface, RegisteredType>( parameters, GlobalTypeSet );
		}

		public void RegisterType<Interface, RegisteredType>( IList<KeyValuePair<Type, object>> parameters ) {
			ContainerHelpers.RegisterType<Interface, RegisteredType>( parameters, TypeSet );
		}

		public static object InvokeGlobalMethod( string methodKey, params object[] parameters ) {
			return ContainerHelpers.InvokeMethod( methodKey, parameters, GlobalMethodSet );
		}

		public object InvokeMethod( string methodKey, params object[] parameters ) {
			string compositeKey = ContainerHelpers.GenerateMethodKey( methodKey, parameters );
			Dictionary<string, MethodContext> targetedSet;
			if ( MethodSet.ContainsKey( compositeKey ) ) {
				targetedSet = MethodSet;
			} else {
				targetedSet = GlobalMethodSet;
			}
			return ContainerHelpers.InvokeMethod( methodKey, parameters, targetedSet );
		}

		public static Interface ResolveGlobalInstance<Interface>() {
			return ContainerHelpers.ResolveInstance<Interface>( GlobalInstanceSet );
		}

		public Interface ResolveInstance<Interface>() {
			Dictionary<Type, object> targetedSet;
			if ( InstanceSet.ContainsKey( typeof( Interface ) ) ) {
				targetedSet = InstanceSet;
			} else {
				targetedSet = GlobalInstanceSet;
			}
			return ContainerHelpers.ResolveInstance<Interface>( targetedSet );
		}

		public static Interface ResolveGlobalType<Interface>() {
			return ContainerHelpers.ResolveType<Interface>( GlobalTypeSet );
		}

		public Interface ResolveType<Interface>() {
			Dictionary<Type, ConstructorContext> targetedSet;
			if ( TypeSet.ContainsKey( typeof( Interface ) ) ) {
				targetedSet = TypeSet;
			} else {
				targetedSet = GlobalTypeSet;
			}
			return ContainerHelpers.ResolveType<Interface>( targetedSet );
		}
	}
}
