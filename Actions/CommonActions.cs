using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Blackboard.PackageAnalyzer.Common;
using D2L.Conversion.Converters.Blackboard.CPC.Plugin;
using Constants = D2L.Conversion.Converters.Blackboard.CPC.Common.Constants;

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

		public static void GroupByPackageVersion( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			XDocument imsmanifest = actionContext.GetImsManifest();
			var resolver = new BlackboardPackageVersionResolver();
			var packageVersion = resolver.GetPackageVersion( actionContext.ExtractedPackageDirectory, imsmanifest );
			string infoPath = Path.Combine( actionContext.ExtractedPackageDirectory, Constants.BB_PACKAGE_INFO_FILENAME );
			string versionInInfo = File.Exists( infoPath )
				? File.ReadAllLines( infoPath ).FirstOrDefault( l => l.Contains( "app.release.number" ) )?.Substring( 19 )
				: "no info file";

			logMessageAction( $"{packageVersion}\t{versionInInfo}\t{actionContext.PackageName}", null );
		}

		public static void EmbeddedLinks( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			var imsmanifest = actionContext.GetImsManifest();

			var elements = imsmanifest
				.XPathSelectElements( $"manifest/resources/resource" );

			foreach( XElement element in elements ) {
				ResourceElement resElement = ResourceElement.FromImsResourceElement( element );
				string resType = element.Attribute( "type" )?.Value;
				string path = Path.Combine( actionContext.ExtractedPackageDirectory, resElement.FileName );

				if(File.Exists( path ) ) {
					string content = File.ReadAllText( path );
					var r = new Regex( @"((@X@Embedded[^/]*?)/)(([^\""])*?\"")" );
					var matches = r.Matches( content );
					foreach( Match match in matches ) {
						logMessageAction( $"{resType}\t{actionContext.PackageName}\t{Path.GetFileName( path )}\t{match.Groups[2]}\t{match.Value}", null );
					}
				}
			}
		}

		public static void GetConsts( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			var resources = actionContext
				.GetManifestResources( "*" )
				.ToArray();
			List<string> res = new List<string>();

			foreach( ResourceElement resource in resources ) {
				string path = Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName );

				if( File.Exists( path ) ) {
					string content = File.ReadAllText( path );
					var r = new Regex( @"(content\.[^\""\@\<]+)" );
					var matches = r.Matches( content );
					foreach( Match match in matches ) {
						res.Add( match.Value );
					}
				}
			}

			foreach( var r in res.Distinct() ) {
				logMessageAction( $"{actionContext.PackageName}\t{r}", null );
			}
		}
	}
}
