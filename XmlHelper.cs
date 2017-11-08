using System.Xml.Linq;

namespace Blackboard.PackageAnalyzer {
	internal static class XmlHelper {

		public static string GetPath( this XElement node ) {
			string path = node.Name.ToString();
			XElement currentNode = node;
			while( currentNode.Parent != null ) {
				currentNode = currentNode.Parent;
				path = currentNode.Name.ToString() + @"\" + path;
			}
			return path;
		}

	}
}
