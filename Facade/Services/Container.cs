using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Facade.Helpers;
using Facade.Interfaces;
using Facade.Models;

namespace Facade.Services
{
    public class Container : IContainer
    {
        private static Dictionary<(Type Type, string Name), object> GlobalInstanceSet { get; set; } = new Dictionary<(Type Type, string Name), object>();

        private static Dictionary<(Type Type, string Name), ConstructorContext> GlobalTypeSet { get; set; } = new Dictionary<(Type Type, string Name), ConstructorContext>();

        private static Dictionary<string, MethodContext> GlobalMethodSet { get; set; } = new Dictionary<string, MethodContext>();

        private Dictionary<(Type Type, string Name), object> InstanceSet { get; set; } = new Dictionary<(Type Type, string Name), object>();

        private Dictionary<(Type Type, string Name), ConstructorContext> TypeSet { get; set; } = new Dictionary<(Type Type, string Name), ConstructorContext>();

        private Dictionary<string, MethodContext> MethodSet { get; set; } = new Dictionary<string, MethodContext>();

        private static string DefaultMappingName { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Registers a default method. 
        /// </summary>
        /// <param name="methodKey">Name for referencing method.</param>
        /// <param name="methodInfo">Provides metadata for the registered method.</param>
        /// <param name="methodOwner">The Target object of the method.  For static methods this parameter should be null.</param>
		public static void RegisterGlobalMethod(string methodKey, MethodInfo methodInfo, object methodOwner)
        {
            ContainerHelpers.RegisterMethod(methodKey, methodInfo, methodOwner, GlobalMethodSet);
        }

        /// <summary>
        /// Registers a method that overrides the registered default method.  
        /// </summary>
        /// <param name="methodKey">Name for referencing method.</param>
        /// <param name="methodInfo">Provides metadata for the registered method.</param>
        /// <param name="methodOwner">The Target object of the method.  For static methods this parameter should be null.</param>
		public void RegisterMethod(string methodKey, MethodInfo methodInfo, object methodOwner)
        {
            ContainerHelpers.RegisterMethod(methodKey, methodInfo, methodOwner, MethodSet);
        }

        /// <summary>
        /// Removes a mapped global method.
        /// </summary>
        /// <param name="methodKey">Mapped method name.</param>
        /// <param name="parameterTypes">Types of all parameters for mapped method.</param>
        public static void RemoveGlobalMethodMapping(string methodKey)
        {
            ContainerHelpers.RemoveMethod(methodKey, GlobalMethodSet);
        }

        /// <summary>
        /// Removes a mapped method.
        /// </summary>
        /// <param name="methodKey">Mapped method name.</param>
        /// <param name="parameterTypes">Types of all parameters for mapped method.</param>
        public void RemoveMethodMapping(string methodKey)
        {
            ContainerHelpers.RemoveMethod(methodKey, MethodSet);
        }

        /// <summary>
        /// Registers a default object instance.
        /// </summary>
        /// <typeparam name="Interface">Interface to map to object instance.</typeparam>
        /// <param name="instance">Object instance to map to Interface</param>
		public static void RegisterGlobalInstance<Interface>(object instance)
        {
            ContainerHelpers.RegisterInstance<Interface>(DefaultMappingName, instance, GlobalInstanceSet);
        }

        /// <summary>
        /// Registers a default object instance.
        /// </summary>
        /// <typeparam name="Interface">Interface to map to object instance.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        /// <param name="instance">Object instance to map to Interface</param>
        public static void RegisterGlobalInstance<Interface>(string name, object instance)
        {
            ContainerHelpers.RegisterInstance<Interface>(name, instance, GlobalInstanceSet);
        }

        /// <summary>
        /// Registers an object instance that overrides the registered default instance.  
        /// </summary>
        /// <typeparam name="Interface">Interface to map to object instance.</typeparam>
        /// <param name="instance">Object instance to map to Interface.</param>
		public void RegisterInstance<Interface>(object instance)
        {
            ContainerHelpers.RegisterInstance<Interface>(DefaultMappingName, instance, InstanceSet);
        }

        /// <summary>
        /// Registers an object instance that overrides the registered default instance.  
        /// </summary>
        /// <typeparam name="Interface">Interface to map to object instance.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        /// <param name="instance">Object instance to map to Interface.</param>
        public void RegisterInstance<Interface>(string name, object instance)
        {
            ContainerHelpers.RegisterInstance<Interface>(name, instance, InstanceSet);
        }

        /// <summary>
        /// Removes a mapped global object instance.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        public static void RemoveGlobalInstanceMapping<Interface>()
        {
            ContainerHelpers.RemoveTypeMapping<Interface, object>(DefaultMappingName, GlobalInstanceSet);
        }

        /// <summary>
        /// Removes a mapped global object instance.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        public static void RemoveGlobalInstanceMapping<Interface>(string name)
        {
            ContainerHelpers.RemoveTypeMapping<Interface, object>(name, GlobalInstanceSet);
        }

        /// <summary>
        /// Removes a mapped object instance.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        public void RemoveInstanceMapping<Interface>()
        {
            ContainerHelpers.RemoveTypeMapping<Interface, object>(DefaultMappingName, InstanceSet);
        }

        /// <summary>
        /// Removes a mapped object instance.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        public void RemoveInstanceMapping<Interface>(string name)
        {
            ContainerHelpers.RemoveTypeMapping<Interface, object>(name, InstanceSet);
        }

        /// <summary>
        /// Registers a default class constructor with parameters.
        /// </summary>
        /// <typeparam name="Interface">Interface to map to class.</typeparam>
        /// <typeparam name="RegisteredType">Class to map to Interface.</typeparam>
        /// <param name="parameters">Parameters to pass to constructor.</param>
		public static void RegisterGlobalType<Interface, RegisteredType>(params object[] parameters)
        {
            ContainerHelpers.RegisterType<Interface, RegisteredType>(DefaultMappingName, parameters, GlobalTypeSet);
        }

        /// <summary>
        /// Registers a default class constructor with parameters.
        /// </summary>
        /// <typeparam name="Interface">Interface to map to class.</typeparam>
        /// <typeparam name="RegisteredType">Class to map to Interface.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        /// <param name="parameters">Parameters to pass to constructor.</param>
        public static void RegisterGlobalType<Interface, RegisteredType>(string name, params object[] parameters)
        {
            ContainerHelpers.RegisterType<Interface, RegisteredType>(name, parameters, GlobalTypeSet);
        }

        /// <summary>
        /// Registers a class constructor with parameters that overrides the default constructor.
        /// </summary>
        /// <typeparam name="Interface">Interface to map to class.</typeparam>
        /// <typeparam name="RegisteredType">Class to map to Interface.</typeparam>
        /// <param name="parameters">Parameters to pass to constructor.</param>
		public void RegisterType<Interface, RegisteredType>(params object[] parameters)
        {
            ContainerHelpers.RegisterType<Interface, RegisteredType>(DefaultMappingName, parameters, TypeSet);
        }

        /// <summary>
        /// Registers a class constructor with parameters that overrides the default constructor.
        /// </summary>
        /// <typeparam name="Interface">Interface to map to class.</typeparam>
        /// <typeparam name="RegisteredType">Class to map to Interface.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        /// <param name="parameters">Parameters to pass to constructor.</param>
        public void RegisterType<Interface, RegisteredType>(string name, params object[] parameters)
        {
            ContainerHelpers.RegisterType<Interface, RegisteredType>(name, parameters, TypeSet);
        }

        /// <summary>
        /// Removes a Global Type Mapping.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        public static void RemoveGlobalTypeMapping<Interface>()
        {
            ContainerHelpers.RemoveTypeMapping<Interface, ConstructorContext>(DefaultMappingName, GlobalTypeSet);
        }

        /// <summary>
        /// Removes a Global Type Mapping.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        public static void RemoveGlobalTypeMapping<Interface>(string name)
        {
            ContainerHelpers.RemoveTypeMapping<Interface, ConstructorContext>(name, GlobalTypeSet);
        }

        /// <summary>
        /// Removes a Type Mapping.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        public void RemoveTypeMapping<Interface>()
        {
            ContainerHelpers.RemoveTypeMapping<Interface, ConstructorContext>(DefaultMappingName, TypeSet);
        }

        /// <summary>
        /// Removes a Type Mapping.
        /// </summary>
        /// <typeparam name="Interface">Mapped Interface.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        public void RemoveTypeMapping<Interface>(string name)
        {
            ContainerHelpers.RemoveTypeMapping<Interface, ConstructorContext>(name, TypeSet);
        }

        /// <summary>
        /// Invokes a default registered method.  
        /// </summary>
        /// <param name="methodKey">Method key.</param>
        /// <param name="parameters">Parameters to pass to method.</param>
        /// <returns>Return value of invoked method.</returns>
		public static object InvokeGlobalMethod(string methodKey, params object[] parameters)
        {
            return ContainerHelpers.InvokeMethod(methodKey, parameters, GlobalMethodSet);
        }

        /// <summary>
        /// Invokes a registered method.  If no method is found, the default method is invoked instead.
        /// </summary>
        /// <param name="methodKey">Method key.</param>
        /// <param name="parameters">Parameters to pass to method.</param>
        /// <returns>Return value of invoked method.</returns>
		public object InvokeMethod(string methodKey, params object[] parameters)
        {
            Dictionary<string, MethodContext> targetedSet;
            if (MethodSet.ContainsKey(methodKey))
            {
                targetedSet = MethodSet;
            }
            else
            {
                targetedSet = GlobalMethodSet;
            }
            return ContainerHelpers.InvokeMethod(methodKey, parameters, targetedSet);
        }

        /// <summary>
        /// Resolves a default object instance.  
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <returns>Registered object instance.</returns>
		public static Interface ResolveGlobalInstance<Interface>()
        {
            return ContainerHelpers.ResolveInstance<Interface>(DefaultMappingName, GlobalInstanceSet);
        }

        /// <summary>
        /// Resolves a default object instance.  
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        /// <returns>Registered object instance.</returns>
        public static Interface ResolveGlobalInstance<Interface>(string name)
        {
            return ContainerHelpers.ResolveInstance<Interface>(name, GlobalInstanceSet);
        }

        /// <summary>
        /// Resolves an object instance.  If no instance is found, the default instance is resolved instead.
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <returns>Registered object instance.</returns>
		public Interface ResolveInstance<Interface>()
        {
            Dictionary<(Type Type, string Name), object> targetedSet;
            if (InstanceSet.ContainsKey((Type: typeof(Interface), Name: DefaultMappingName)))
            {
                targetedSet = InstanceSet;
            }
            else
            {
                targetedSet = GlobalInstanceSet;
            }
            return ContainerHelpers.ResolveInstance<Interface>(DefaultMappingName, targetedSet);
        }

        /// <summary>
        /// Resolves an object instance.  If no instance is found, the default instance is resolved instead.
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        /// <returns>Registered object instance.</returns>
        public Interface ResolveInstance<Interface>(string name)
        {
            Dictionary<(Type Type, string Name), object> targetedSet;
            if (InstanceSet.ContainsKey((Type: typeof(Interface), Name: name)))
            {
                targetedSet = InstanceSet;
            }
            else
            {
                targetedSet = GlobalInstanceSet;
            }
            return ContainerHelpers.ResolveInstance<Interface>(name, targetedSet);
        }

        /// <summary>
        /// Resolves a default class constructor.
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <returns>Instance of Registered Class</returns>
		public static Interface ResolveGlobalType<Interface>()
        {
            return ContainerHelpers.ResolveType<Interface>(DefaultMappingName, GlobalTypeSet);
        }

        /// <summary>
        /// Resolves a default class constructor.
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        /// <returns>Instance of Registered Class</returns>
        public static Interface ResolveGlobalType<Interface>(string name)
        {
            return ContainerHelpers.ResolveType<Interface>(name, GlobalTypeSet);
        }

        /// <summary>
        /// Resolves a class constructor.  If no constructor is found, the default constructor will be resolved instead.  
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <returns>Instance of Registered Class</returns>
		public Interface ResolveType<Interface>()
        {
            Dictionary<(Type Type, string Name), ConstructorContext> targetedSet;
            if (TypeSet.ContainsKey((Type: typeof(Interface), Name: DefaultMappingName)))
            {
                targetedSet = TypeSet;
            }
            else
            {
                targetedSet = GlobalTypeSet;
            }
            return ContainerHelpers.ResolveType<Interface>(DefaultMappingName, targetedSet);
        }

        /// <summary>
        /// Resolves a class constructor.  If no constructor is found, the default constructor will be resolved instead.  
        /// </summary>
        /// <typeparam name="Interface">Registered Interface.</typeparam>
        /// <param name="name">Name of the mapping.</param>
        /// <returns>Instance of Registered Class</returns>
        public Interface ResolveType<Interface>(string name)
        {
            Dictionary<(Type Type, string Name), ConstructorContext> targetedSet;
            if (TypeSet.ContainsKey((Type: typeof(Interface), Name: name)))
            {
                targetedSet = TypeSet;
            }
            else
            {
                targetedSet = GlobalTypeSet;
            }
            return ContainerHelpers.ResolveType<Interface>(name, targetedSet);
        }
    }
}
