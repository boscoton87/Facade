using System.Reflection;

namespace Facade.Models {
	internal class MethodContext {
		internal MethodInfo MethodInfo { get; }

		internal object MethodOwner { get; }

		internal MethodContext( MethodInfo methodInfo, object methodOwner ) {
			MethodInfo = methodInfo;
			MethodOwner = methodOwner;
		}
	}
}
