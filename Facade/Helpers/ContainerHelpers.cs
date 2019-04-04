using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Facade.Exceptions;
using Facade.Models;

namespace Facade.Helpers
{
    internal static class ContainerHelpers {
		internal static void RegisterInstance<Interface>( object instance, Dictionary<Type, object> register ) {
			Type interfaceType = typeof( Interface );
			ValidateInterface( interfaceType );
            CheckMappingAvailability<Interface, object>(register);
            if ( ( instance is Interface ) == false ) {
				throw new InvalidTypeMappingException( $"Instance parameter provided does not implement the {nameof( Interface )} interface." );
			}
			register.Add( interfaceType, instance );
		}

		internal static void RegisterType<Interface, RegisteredType>( object[] parameters, Dictionary<Type, ConstructorContext> register ) {
			Type interfaceType = typeof( Interface );
			Type registeredType = typeof( RegisteredType );
			ValidateInterface( interfaceType );
            CheckMappingAvailability<Interface, ConstructorContext>(register);
            if ( registeredType.GetInterface( typeof( Interface ).Name ) == null ) {
				throw new InvalidTypeMappingException( $"{nameof( registeredType )} does not implement {nameof( Interface )}." );
			}
			Type[] types = parameters.Select( param => param.GetType() ).ToArray();
			if ( registeredType.GetConstructor( types ) == null ) {
				throw new InvalidTypeMappingException( $"{nameof( registeredType )} does not have a constructor that accepts the supplied parameters." );
			}
			register.Add( interfaceType, new ConstructorContext( registeredType, parameters ) );
		}

		internal static void RegisterMethod( string methodKey, MethodInfo methodInfo, object methodOwner, Dictionary<string, MethodContext> register ) {
			if ( string.IsNullOrWhiteSpace(methodKey) ) {
				throw new InvalidArgumentException( "methodKey is invalid." );
			} else if ( register.ContainsKey(methodKey) ) {
				throw new MappingTakenException( $"a method has already been registered to {methodKey}." );
			} else if ( methodInfo == null ) {
				throw new ArgumentNullException( "methodInfo must not be null." );
			}
			register.Add(methodKey, new MethodContext( methodInfo, methodOwner ) );
		}

        internal static void RemoveMethod(string methodKey, Dictionary<string, MethodContext> register)
        {
            register.Remove(methodKey);
        }

        internal static void RemoveTypeMapping<Interface, ValueType>(Dictionary<Type, ValueType> register)
        {
            register.Remove(typeof(Interface));
        }

        internal static Interface ResolveInstance<Interface>( Dictionary<Type, object> register ) {
			Type interfaceType = typeof( Interface );
			if ( register.ContainsKey( interfaceType ) == false ) {
				throw new NoMappingException( $"An instance of {nameof( Interface )} is not registered." );
			}
			return ( Interface ) register[ interfaceType ];
		}

		internal static Interface ResolveType<Interface>( Dictionary<Type, ConstructorContext> register ) {
			Type interfaceType = typeof( Interface );
			if ( register.ContainsKey( interfaceType ) == false ) {
				throw new NoMappingException( $"A TypeMapping of {nameof( Interface )} is not registered." );
			}
			var registeredType = register[ interfaceType ];
			Type[] types = registeredType.Parameters.Select( param => param.GetType() ).ToArray();
			return ( Interface ) register[ interfaceType ].BuiltType.GetConstructor( types ).Invoke(registeredType.Parameters.ToArray());
		}

		internal static object InvokeMethod( string methodKey, object[] parameters, Dictionary<string, MethodContext> register ) {
			if ( register.ContainsKey(methodKey) == false ) {
				throw new NoMappingException( $"No method mapped to {methodKey}" );
			}
			var methodInfo = register[methodKey].MethodInfo;
			object owner = register[methodKey].MethodOwner;
			try {
				return methodInfo.Invoke( owner, parameters );
			} catch ( Exception ) {
				throw new MethodInvocationException( $"The method mapped to {methodKey} failed to execute." );
			}
		}

        private static void CheckMappingAvailability<Interface, MappedType>(Dictionary<Type, MappedType> register)
        {
            if (register.ContainsKey(typeof(Interface)))
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
