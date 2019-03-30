using System;
using System.Collections.Generic;

namespace Facade.Models {
	internal class ConstructorContext {
		internal Type BuiltType { get; }

		internal IList<KeyValuePair<Type, object>> Parameters { get; }

		internal ConstructorContext(Type classType, IList<KeyValuePair<Type, object>> parameters ) {
			BuiltType = classType;
			Parameters = parameters;
		}
	}
}
