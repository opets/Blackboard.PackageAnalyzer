using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using D2L.Conversion.Converters.Blackboard.CPC.Common;

namespace D2L.Conversion.Converters.Blackboard.CPC.Plugin {

	internal sealed class BlackboardPackageVersionResolver {

		private const string Release_Property_Name = "app.release.number";

		public string GetPackageVersion( string packageRootDirectoryPath, XDocument imsManifest ) {

			var namespaces = imsManifest.Root.Attributes()
				.Where( a => a.Value == Constants.BlackboardNamespace );

			if( namespaces.Any() ) {
				return GetVersionInfo( packageRootDirectoryPath );
			}

			// Blackboard 6.0;
			var xmlNamespaceManager = new XmlNamespaceManager( new NameTable() );
			xmlNamespaceManager.AddNamespace( "bbcp", "http://www.blackboard.com/content-packaging" ); // no slash

			var bbfileElement = imsManifest.XPathSelectElement( "/manifest/resources/resource[@bbcp:file != '']", xmlNamespaceManager );
			if( bbfileElement != null ) {
				return PackageVersion.V_6_0;
			}

			var tableofcontentsElement = imsManifest.XPathSelectElement( "/manifest/organizations/tableofcontents" )
				?? imsManifest.XPathSelectElement( "/manifest/organizations/organization/tableofcontents" );
			if( tableofcontentsElement != null ) {
				return PackageVersion.V_5_5;
			}


			return PackageVersion.Unsupported;
		}

		private string GetVersionInfo( string packageRootDirectoryPath ) {
			string infoFilename = Path.Combine( packageRootDirectoryPath, Constants.BB_PACKAGE_INFO_FILENAME );

			if( !File.Exists( infoFilename ) ) {
				return PackageVersion.V_6_0;
			}

			string infoFileContents = File.ReadAllText( infoFilename );

			if( infoFileContents.Contains( Release_Property_Name + "=9.0" ) ) {
				return PackageVersion.V_9_0;
			}

			if( IsBlackboard_9p1( infoFileContents ) ) {
				return PackageVersion.V_9_1;
			}

			return PackageVersion.V_6_0;
		}

		internal static bool IsBlackboard_9p1( string infoFileContents ) {
			//Matches 'app.release.number=9.1' and 'app.release.number=xxxx.'
			string pattern = Release_Property_Name.Replace( ".", "\\." )
							+ @"=(?:(?:(?'ver'\d{4})\.)|(?:9\.1))";

			Match match = Regex.Match( infoFileContents, pattern );
			if( match.Success ) {
				Group ver = match.Groups["ver"];
				if( ver.Success ) { //new 9.1 app.release.number=xxxx
					return int.Parse( match.Groups["ver"].Value ) >= 3000;
				}
				return true; //old 9.1 app.release.number=9.1
			}
			return false; //not 9.1
		}
	}
}
