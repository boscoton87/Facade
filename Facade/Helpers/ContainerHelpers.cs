using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Facade.Exceptions;
using Facade.Models;

namespace Facade.Helpers
{
    internal static class ContainerHelpers
    {
        internal static void RegisterInstance<Interface>(string name, object instance, Dictionary<(Type Type, string Name), object> register)
        {
            Type interfaceType = typeof(Interface);
            ValidateInterface(interfaceType);
            CheckMappingAvailability<Interface, object>(name, register);
            if ((instance is Interface) == false)
            {
                throw new InvalidTypeMappingException($"Instance parameter provided does not implement the {nameof(Interface)} interface.");
            }
            register.Add((Type: interfaceType, Name: name), instance);
        }

        internal static void RegisterType<Interface, RegisteredType>(string name, object[] parameters, Dictionary<(Type Type, string Name), ConstructorContext> register)
        {
            Type interfaceType = typeof(Interface);
            Type registeredType = typeof(RegisteredType);
            ValidateInterface(interfaceType);
            CheckMappingAvailability<Interface, ConstructorContext>(name, register);
            if (registeredType.GetInterface(typeof(Interface).Name) == null)
            {
                throw new InvalidTypeMappingException($"{nameof(registeredType)} does not implement {nameof(Interface)}.");
            }
            Type[] types = parameters.Select(param => param.GetType()).ToArray();
            if (registeredType.GetConstructor(types) == null)
            {
                throw new InvalidTypeMappingException($"{nameof(registeredType)} does not have a constructor that accepts the supplied parameters.");
            }
            register.Add((Type: interfaceType, Name: name), new ConstructorContext(registeredType, parameters));
        }

        internal static void RegisterMethod(string methodKey, MethodInfo methodInfo, object methodOwner, Dictionary<string, MethodContext> register)
        {
            if (string.IsNullOrWhiteSpace(methodKey))
            {
                throw new InvalidArgumentException("methodKey is invalid.");
            }
            else if (register.ContainsKey(methodKey))
            {
                throw new MappingTakenException($"a method has already been registered to {methodKey}.");
            }
            else if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo must not be null.");
            }
            else if (methodInfo.IsGenericMethod)
            {
                throw new InvalidArgumentException("Generic Methods are not supported.");
            }
            register.Add(methodKey, new MethodContext(methodInfo, methodOwner));
        }

        internal static void RemoveMethod(string methodKey, Dictionary<string, MethodContext> register)
        {
            register.Remove(methodKey);
        }

        internal static void RemoveTypeMapping<Interface, ValueType>(string name, Dictionary<(Type Type, string Name), ValueType> register)
        {
            register.Remove((Type: typeof(Interface), Name: name));
        }

        internal static Interface ResolveInstance<Interface>(string name, Dictionary<(Type Type, string Name), object> register)
        {
            Type interfaceType = typeof(Interface);
            if (register.ContainsKey((Type: interfaceType, Name: name)) == false)
            {
                throw new NoMappingException($"An instance of {nameof(Interface)} is not registered.");
            }
            return (Interface)register[(Type: interfaceType, Name: name)];
        }

        internal static Interface ResolveType<Interface>(string name, Dictionary<(Type Type, string Name), ConstructorContext> register)
        {
            Type interfaceType = typeof(Interface);
            if (register.ContainsKey((Type: interfaceType, Name: name)) == false)
            {
                throw new NoMappingException($"A TypeMapping of {nameof(Interface)} is not registered.");
            }
            var registeredType = register[(Type: interfaceType, Name: name)];
            Type[] types = registeredType.Parameters.Select(param => param.GetType()).ToArray();
            return (Interface)register[(Type: interfaceType, Name: name)].BuiltType.GetConstructor(types).Invoke(registeredType.Parameters.ToArray());
        }

        internal static object InvokeMethod(string methodKey, object[] parameters, Dictionary<string, MethodContext> register)
        {
            if (register.ContainsKey(methodKey) == false)
            {
                throw new NoMappingException($"No method mapped to {methodKey}");
            }
            var methodInfo = register[methodKey].MethodInfo;
            object owner = register[methodKey].MethodOwner;
            try
            {
                return methodInfo.Invoke(owner, parameters);
            }
            catch (Exception)
            {
                throw new MethodInvocationException($"The method mapped to {methodKey} failed to execute.");
            }
        }

        private static void CheckMappingAvailability<Interface, MappedType>(string name, Dictionary<(Type Type, string Name), MappedType> register)
        {
            if (register.ContainsKey((Type: typeof(Interface), Name: name)))
            {
                throw new MappingTakenException($"{nameof(Interface)} already has been registered.");
            }
        }

        private static void ValidateInterface(Type interfaceType)
        {
            if (interfaceType.IsInterface == false)
            {
                throw new InvalidTypeMappingException($"{nameof(interfaceType)} is not an Interface.");
            }
        }
    }
}
