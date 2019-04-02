using System.Reflection;

namespace Facade.Interfaces
{
    public interface IRegister {
		void RegisterInstance<Interface>( object instance );

		void RegisterType<Interface, RegisteredType>( params object[] parameters );

		void RegisterMethod( string methodKey, MethodInfo methodInfo, object methodOwner );
	}
}
