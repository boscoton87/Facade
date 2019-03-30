using System.Collections.Generic;

namespace Facade.Interfaces {
	public interface IResolver {
		Interface ResolveInstance<Interface>();

		Interface ResolveType<Interface>();

		object InvokeMethod( string methodKey, params object[] parameters );
	}
}
