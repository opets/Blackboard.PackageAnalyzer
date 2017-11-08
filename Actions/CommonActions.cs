using System;
using System.Linq;
using System.Xml.XPath;

namespace Blackboard.PackageAnalyzer.Actions {
	internal static class CommonActions {

		public static void NoImsManifest( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			if( actionContext.GetImsManifest() == null ) {
				logMessageAction( $"{actionContext.PackageName}", null );
			}
		}

		public static void AvailableResourceTypes( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			var imsmanifest = actionContext.GetImsManifest();

			var types = imsmanifest
				.XPathSelectElements( $"manifest/resources/resource" )
				.Select( e => e.Attribute( "type" ).Value )
				.Distinct();

			foreach( var attribute in types ) {
				logMessageAction( attribute, null );
			}
		}


	}
}
