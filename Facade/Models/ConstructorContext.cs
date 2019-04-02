using System;
using System.Collections.Generic;

namespace Facade.Models {
	internal class ConstructorContext {
		internal Type BuiltType { get; }

		internal IList<object> Parameters { get; }

		internal ConstructorContext(Type classType, IList<object> parameters ) {
			BuiltType = classType;
			Parameters = parameters;
		}
	}
}
