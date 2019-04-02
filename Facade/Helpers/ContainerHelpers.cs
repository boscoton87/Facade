using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Facade.Models;

namespace Facade.Helpers {
	internal static class ContainerHelpers {
		internal static void RegisterInstance<Interface>( object instance, Dictionary<Type, object> register ) {
			Type interfaceType = typeof( Interface );
			ValidateInterface( interfaceType );
			if ( ( instance is Interface ) == false ) {
				throw new Exception( $"Instance parameter provided does not implement the {nameof( Interface )} interface." );
			}
			register.Add( interfaceType, instance );
		}

		internal static void RegisterType<Interface, RegisteredType>( IList<KeyValuePair<Type, object>> parameters, Dictionary<Type, ConstructorContext> register ) {
			Type interfaceType = typeof( Interface );
			Type registeredType = typeof( RegisteredType );
			ValidateInterface( interfaceType );
			if ( registeredType.GetInterface( typeof( Interface ).Name ) == null ) {
				throw new Exception( $"{nameof( registeredType )} does not implement {nameof( Interface )}." );
			}
			Type[] types = parameters.Select( param => param.Key ).ToArray();
			if ( registeredType.GetConstructor( types ) == null ) {
				throw new Exception( $"{nameof( registeredType )} does not have a constructor that accepts the supplied parameters." );
			}
			register.Add( interfaceType, new ConstructorContext( registeredType, parameters ) );
		}

		internal static void RegisterMethod( string methodKey, MethodInfo methodInfo, object methodOwner, Dictionary<string, MethodContext> register ) {
			if ( methodInfo == null ) {
				throw new Exception( "methodInfo must not be null." );
			}
			var methodParams = methodInfo.GetParameters().Select( param => param.ParameterType.FullName ).ToArray();
			string compositeKey = GenerateMethodKey( methodKey, methodParams );
			if ( string.IsNullOrWhiteSpace( compositeKey ) ) {
				throw new Exception( "methodKey is invalid." );
			} else if ( register.ContainsKey( compositeKey ) ) {
				throw new Exception( $"a method has already been registered to {compositeKey}." );
			} else if ( methodInfo == null ) {
				throw new Exception( "methodInfo must not be null." );
			}
			register.Add( compositeKey, new MethodContext( methodInfo, methodOwner ) );
		}

        internal static void RemoveMethod(string methodKey, Type[] parameterTypes, Dictionary<string, MethodContext> register)
        {
            string compositeKey = GenerateMethodKey(methodKey, parameterTypes);
            register.Remove(compositeKey);
        }

        internal static void RemoveTypeMapping<Interface, ValueType>(Dictionary<Type, ValueType> register)
        {
            register.Remove(typeof(Interface));
        }

        internal static Interface ResolveInstance<Interface>( Dictionary<Type, object> register ) {
			Type interfaceType = typeof( Interface );
			if ( register.ContainsKey( interfaceType ) == false ) {
				throw new Exception( $"An instance of {nameof( Interface )} is not registered." );
			}
			return ( Interface ) register[ interfaceType ];
		}

		internal static Interface ResolveType<Interface>( Dictionary<Type, ConstructorContext> register ) {
			Type interfaceType = typeof( Interface );
			if ( register.ContainsKey( interfaceType ) == false ) {
				throw new Exception( $"A TypeMapping of {nameof( Interface )} is not registered." );
			}
			var registeredType = register[ interfaceType ];
			Type[] types = registeredType.Parameters.Select( param => param.Key ).ToArray();
			object[] parameters = registeredType.Parameters.Select( param => param.Value ).ToArray();
			return ( Interface ) register[ interfaceType ].BuiltType.GetConstructor( types ).Invoke( parameters );
		}

		internal static object InvokeMethod( string methodKey, object[] parameters, Dictionary<string, MethodContext> register ) {
			string compositeKey = GenerateMethodKey( methodKey, parameters );
			if ( register.ContainsKey( compositeKey ) == false ) {
				throw new Exception( $"No method mapped to {compositeKey}" );
			}
			var methodInfo = register[ compositeKey ].MethodInfo;
			object owner = register[ compositeKey ].MethodOwner;
			try {
				return methodInfo.Invoke( owner, parameters );
			} catch ( TargetInvocationException ) {
				throw new Exception( $"The method mapped to {compositeKey} failed to execute." );
			}
		}

		internal static string GenerateMethodKey( string methodKey, object[] parameters ) {
			StringBuilder keyBuilder = new StringBuilder( methodKey );
			parameters.ToList().ForEach( parameter => keyBuilder.Append( $"_{parameter.GetType()}" ) );
			return keyBuilder.ToString();
		}

        internal static string GenerateMethodKey(string methodKey, Type[] parameterTypes)
        {
            StringBuilder keyBuilder = new StringBuilder(methodKey);
            parameterTypes.ToList().ForEach(type => keyBuilder.Append($"_{nameof(type)}"));
            return keyBuilder.ToString();
        }

        private static void ValidateInterface( Type interfaceType ) {
			if ( interfaceType.IsInterface == false ) {
				throw new Exception( $"{nameof( interfaceType )} is not an Interface." );
			}
		}
	}
}
