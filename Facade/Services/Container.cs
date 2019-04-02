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

        /// <summary>
        /// Registers a default method. 
        /// </summary>
        /// <param name="methodKey">Name for referencing method.</param>
        /// <param name="methodInfo">Provides metadata for the registered method.</param>
        /// <param name="methodOwner">The Target object of the method.  For static methods this parameter should be null.</param>
		public static void RegisterGlobalMethod( string methodKey, MethodInfo methodInfo, object methodOwner ) {
			ContainerHelpers.RegisterMethod( methodKey, methodInfo, methodOwner, GlobalMethodSet );
		}

        /// <summary>
        /// Registers a method that overrides the registered default method.  
        /// </summary>
        /// <param name="methodKey">Name for referencing method.</param>
        /// <param name="methodInfo">Provides metadata for the registered method.</param>
        /// <param name="methodOwner">The Target object of the method.  For static methods this parameter should be null.</param>
		public void RegisterMethod( string methodKey, MethodInfo methodInfo, object methodOwner ) {
			ContainerHelpers.RegisterMethod( methodKey, methodInfo, methodOwner, MethodSet );
		}

        /// <summary>
        /// Removes a mapped global method.
        /// </summary>
        /// <param name="methodKey">Mapped method name.</param>
        /// <param name="parameterTypes">Types of all parameters for mapped method.</param>
        public static void RemoveGlobalMethodMapping(string methodKey, params Type[] parameterTypes)
        {
            ContainerHelpers.RemoveMethod(methodKey, parameterTypes, GlobalMethodSet);
        }

        /// <summary>
        /// Removes a mapped method.
        /// </summary>
        /// <param name="methodKey">Mapped method name.</param>
        /// <param name="parameterTypes">Types of all parameters for mapped method.</param>
        public void RemoveMethodMapping(string methodKey, params Type[] parameterTypes)
        {
            ContainerHelpers.RemoveMethod(methodKey, parameterTypes, MethodSet);
        }

        /// <summary>
        /// Registers a default object instance.
        /// </summary>
        /// <typeparam name="Interface">Interface to map to object instance.</typeparam>
        /// <param name="instance">Object instance to map to Interface</param>
		public static void RegisterGlobalInstance<Interface>( object instance ) {
			ContainerHelpers.RegisterInstance<Interface>( instance, GlobalInstanceSet );
		}

        /// <summary>
        /// Registers an object instance that overrides the registered default instance.  
        /// </summary>
        /// <typeparam name="Interface">Interface to map to object instance.</typeparam>
        /// <param name="instance">Object instance to map to Interface.</param>
		public void RegisterInstance<Interface>( object instance ) {
			ContainerHelpers.RegisterInstance<Interface>( instance, InstanceSet );
		}

        /// <summary>
        /// Removes a mapped global object instance.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        public static void RemoveGlobalInstanceMapping<Interface>()
        {
            ContainerHelpers.RemoveTypeMapping<Interface, object>(GlobalInstanceSet);
        }

        /// <summary>
        /// Removes a mapped object instance.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        public void RemoveInstanceMapping<Interface>()
        {
            ContainerHelpers.RemoveTypeMapping<Interface, object>(InstanceSet);
        }

        /// <summary>
        /// Registers a default class constructor.
        /// </summary>
        /// <typeparam name="Interface">Interface to map to class.</typeparam>
        /// <typeparam name="RegisteredType">Class to map to Interface.</typeparam>
		public static void RegisterGlobalType<Interface, RegisteredType>() {
			ContainerHelpers.RegisterType<Interface, RegisteredType>( new List<KeyValuePair<Type, object>>(), GlobalTypeSet );
		}

        /// <summary>
        /// Registers a class constructor that overrides the registered default constructor.
        /// </summary>
        /// <typeparam name="Interface">Interface to map to class.</typeparam>
        /// <typeparam name="RegisteredType">Class to map to Interface.</typeparam>
		public void RegisterType<Interface, RegisteredType>() {
			ContainerHelpers.RegisterType<Interface, RegisteredType>( new List<KeyValuePair<Type, object>>(), TypeSet );
		}

        /// <summary>
        /// Registers a default class constructor with parameters.
        /// </summary>
        /// <typeparam name="Interface">Interface to map to class.</typeparam>
        /// <typeparam name="RegisteredType">Class to map to Interface.</typeparam>
        /// <param name="parameters">Parameters to pass to constructor.</param>
		public static void RegisterGlobalType<Interface, RegisteredType>( IList<KeyValuePair<Type, object>> parameters ) {
			ContainerHelpers.RegisterType<Interface, RegisteredType>( parameters, GlobalTypeSet );
		}

        /// <summary>
        /// Registers a class constructor with parameters that overrides the default constructor.
        /// </summary>
        /// <typeparam name="Interface">Interface to map to class.</typeparam>
        /// <typeparam name="RegisteredType">Class to map to Interface.</typeparam>
        /// <param name="parameters">Parameters to pass to constructor.</param>
		public void RegisterType<Interface, RegisteredType>( IList<KeyValuePair<Type, object>> parameters ) {
			ContainerHelpers.RegisterType<Interface, RegisteredType>( parameters, TypeSet );
		}

        /// <summary>
        /// Removes a Global Type Mapping.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        public static void RemoveGlobalTypeMapping<Interface>()
        {
            ContainerHelpers.RemoveTypeMapping<Interface, ConstructorContext>(GlobalTypeSet);
        }

        /// <summary>
        /// Removes a Type Mapping.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        public void RemoveTypeMapping<Interface>()
        {
            ContainerHelpers.RemoveTypeMapping<Interface, ConstructorContext>(TypeSet);
        }

        /// <summary>
        /// Invokes a default registered method.  
        /// </summary>
        /// <param name="methodKey">Method key.</param>
        /// <param name="parameters">Parameters to pass to method.</param>
        /// <returns>Return value of invoked method.</returns>
		public static object InvokeGlobalMethod( string methodKey, params object[] parameters ) {
			return ContainerHelpers.InvokeMethod( methodKey, parameters, GlobalMethodSet );
		}

        /// <summary>
        /// Invokes a registered method.  If no method is found, the default method is invoked instead.
        /// </summary>
        /// <param name="methodKey">Method key.</param>
        /// <param name="parameters">Parameters to pass to method.</param>
        /// <returns>Return value of invoked method.</returns>
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

        /// <summary>
        /// Resolves a default object instance.  
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <returns>Registered object instance.</returns>
		public static Interface ResolveGlobalInstance<Interface>() {
			return ContainerHelpers.ResolveInstance<Interface>( GlobalInstanceSet );
		}

        /// <summary>
        /// Resolves an object instance.  If no instance is found, the default instance is resolved instead.
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <returns>Registered object instance.</returns>
		public Interface ResolveInstance<Interface>() {
			Dictionary<Type, object> targetedSet;
			if ( InstanceSet.ContainsKey( typeof( Interface ) ) ) {
				targetedSet = InstanceSet;
			} else {
				targetedSet = GlobalInstanceSet;
			}
			return ContainerHelpers.ResolveInstance<Interface>( targetedSet );
		}

        /// <summary>
        /// Resolves a default class constructor.
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <returns>Instance of Registered Class</returns>
		public static Interface ResolveGlobalType<Interface>() {
			return ContainerHelpers.ResolveType<Interface>( GlobalTypeSet );
		}

        /// <summary>
        /// Resolves a class constructor.  If no constructor is found, the default constructor will be resolved instead.  
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <returns>Instance of Registered Class</returns>
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
