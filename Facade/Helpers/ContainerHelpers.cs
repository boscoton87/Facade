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
        internal static void RegisterInstance<KeyType>(string name, object instance, Dictionary<(Type Type, string Name), object> register)
        {
            Type keyType = typeof(KeyType);
            CheckMappingAvailability<KeyType, object>(name, register);
            if ((instance is KeyType) == false)
            {
                throw new InvalidTypeMappingException($"Instance parameter provided does not implement the {nameof(KeyType)} interface.");
            }
            register.Add((Type: keyType, Name: name), instance);
        }

        internal static void RegisterType<KeyType, RegisteredType>(string name, object[] parameters, Dictionary<(Type Type, string Name), ConstructorContext> register)
        {
            Type keyType = typeof(KeyType);
            Type registeredType = typeof(RegisteredType);
            CheckMappingAvailability<KeyType, ConstructorContext>(name, register);
            if (!keyType.IsAssignableFrom(registeredType))
            {
                throw new InvalidTypeMappingException($"{nameof(registeredType)} does not implement {nameof(KeyType)}.");
            }
            Type[] types = parameters.Select(param => param.GetType()).ToArray();
            if (registeredType.GetConstructor(types) == null)
            {
                throw new InvalidTypeMappingException($"{nameof(registeredType)} does not have a constructor that accepts the supplied parameters.");
            }
            register.Add((Type: keyType, Name: name), new ConstructorContext(registeredType, parameters));
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

        internal static void RemoveTypeMapping<KeyType, ValueType>(string name, Dictionary<(Type Type, string Name), ValueType> register)
        {
            register.Remove((Type: typeof(KeyType), Name: name));
        }

        internal static KeyType ResolveInstance<KeyType>(string name, Dictionary<(Type Type, string Name), object> register)
        {
            Type keyType = typeof(KeyType);
            if (register.ContainsKey((Type: keyType, Name: name)) == false)
            {
                throw new NoMappingException($"An instance of {nameof(KeyType)} is not registered.");
            }
            return (KeyType)register[(Type: keyType, Name: name)];
        }

        internal static KeyType ResolveType<KeyType>(string name, Dictionary<(Type Type, string Name), ConstructorContext> register)
        {
            Type keyType = typeof(KeyType);
            if (register.ContainsKey((Type: keyType, Name: name)) == false)
            {
                throw new NoMappingException($"A TypeMapping of {nameof(KeyType)} is not registered.");
            }
            var registeredType = register[(Type: keyType, Name: name)];
            Type[] types = registeredType.Parameters.Select(param => param.GetType()).ToArray();
            return (KeyType)register[(Type: keyType, Name: name)].BuiltType.GetConstructor(types).Invoke(registeredType.Parameters.ToArray());
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

        private static void CheckMappingAvailability<KeyType, MappedType>(string name, Dictionary<(Type Type, string Name), MappedType> register)
        {
            if (register.ContainsKey((Type: typeof(KeyType), Name: name)))
            {
                throw new MappingTakenException($"{nameof(KeyType)} already has been registered.");
            }
        }
    }
}
