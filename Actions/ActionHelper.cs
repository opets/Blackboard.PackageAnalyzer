using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Blackboard.PackageAnalyzer.Common;

namespace Blackboard.PackageAnalyzer.Actions {

	internal static class ActionHelper {

		public static XDocument GetImsManifest( this ActionContext actionContext ) {
			string imsmanifestPath = Path.Combine( actionContext.ExtractedPackageDirectory, "imsmanifest.xml" );
			return GetDocument( imsmanifestPath );
		}

		public static IEnumerable<ResourceElement> GetManifestResources( this ActionContext actionContext, params string[] resourceTypes ) {
			XDocument imsmanifest = actionContext.GetImsManifest();

			if( imsmanifest == null || resourceTypes == null ) {
					return new ResourceElement[0];
			}

			if( resourceTypes.Length == 1 && resourceTypes[0] == "*" ) {
				return imsmanifest.XPathSelectElements( "manifest/resources/resource" )
					.Select( ResourceElement.FromImsResourceElement );
			}

			return resourceTypes
					.SelectMany( resourceType => imsmanifest.XPathSelectElements( $"manifest/resources/resource[@type='{resourceType}']" ) )
					.Select( ResourceElement.FromImsResourceElement );
		}

		public static XDocument GetDocument( string path ) {
			if( !File.Exists( path ) ) {
				return null;
			}

			using( FileStream fileStream = new FileStream( path, FileMode.Open ) ) {
				return XDocument.Load( fileStream );
			}
		}
	}
}
