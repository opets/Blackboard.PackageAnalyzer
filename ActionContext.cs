using System.IO;

namespace Blackboard.PackageAnalyzer {

	internal class ActionContext {

		public string PackageFilePath { get; }

		public string PackageName => Path.GetFileName( PackageFilePath );

		public string ExtractedPackageDirectory { get; }

		public ActionContext( string packageFilePath, string extractedPackageDirectory ) {
			PackageFilePath = packageFilePath;
			ExtractedPackageDirectory = extractedPackageDirectory;
		}

	}
}
