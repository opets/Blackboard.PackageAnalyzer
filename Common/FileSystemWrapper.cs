using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Blackboard.PackageAnalyzer.Common {
	public sealed class FileSystemWrapper {

		public IEnumerable<string> EnumerateDirectories(
			string path,
			string pattern = "*",
			SearchOption option = SearchOption.TopDirectoryOnly
		) =>
			string.IsNullOrWhiteSpace( path ) ?
				Enumerable.Empty<string>() :
				Directory.EnumerateDirectories( path, pattern, option );
	}
}
