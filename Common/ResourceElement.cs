﻿using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Blackboard.PackageAnalyzer.Common {

	internal sealed class ResourceElement {

		public ResourceElement( string identifier, string title = null, string fileName = null, string resourceType= null ) {
			FileName = fileName;
			ResourceType = resourceType;
			Title = title;
			Identifier = identifier;
		}

		public string FileName { get; }

		public string Title { get; }

		public string Identifier { get; }
		public string ResourceType { get; }

		public static ResourceElement FromImsResourceElement( XElement e ) {
			return new ResourceElement(
				e.Attribute( "identifier" )?.Value,
				ProcessTitle( e.Attribute( XName.Get( "title", Constants.BlackboardNamespace ) )?.Value ),
				e.Attribute( XName.Get( "file", Constants.BlackboardNamespace ) )?.Value,
				e.Attribute( "type" )?.Value				
			);
		}

		private static string ProcessTitle( string title )
			=> string.IsNullOrEmpty( title ) ? title : Regex.Replace( title, @"<[^>]*>", string.Empty );

	}
}
