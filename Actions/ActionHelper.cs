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

			return imsmanifest == null
				? new ResourceElement[0]
				: resourceTypes
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
