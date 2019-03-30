using System;
using System.Collections.Generic;
using System.Reflection;

namespace Facade.Interfaces {
	public interface IRegister {
		void RegisterInstance<Interface>( object instance );

		void RegisterType<Interface, RegisteredType>();

		void RegisterType<Interface, RegisteredType>( IList<KeyValuePair<Type, object>> parameters );

		void RegisterMethod( string methodKey, MethodInfo methodInfo, object methodOwner );
	}
}
