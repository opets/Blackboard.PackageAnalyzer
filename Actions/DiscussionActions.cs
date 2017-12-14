using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Blackboard.PackageAnalyzer.Actions {
	internal static class DiscussionActions {


		public static void DiscussionWithFlag( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			const string flag = "FORUM/POSTFIRST";
			const string excludePropValue = "NO_POST_FIRST";

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "resource/x-bb-discussionboard" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();


			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				string propValue = doc.XPathSelectElement( flag )?.Attribute( "value" )?.Value;
				if( propValue != excludePropValue ) {
					logMessageAction( $"{propValue}\t{actionContext.PackageName}\t{Path.GetFileName( path )}", null );
				}
			}
		}

		public static void DiscussionMessageWithFlag( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			const string flag = "POSTFIRST";
			const string excludePropValue = "false";

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "resource/x-bb-discussionboard" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();


			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				string forumValue = doc.XPathSelectElement( "FORUM/POSTFIRST" )?.Attribute( "value" )?.Value;

				var messages = doc.XPathSelectElements( "FORUM/MESSAGETHREADS/MSG" )
					.ToArray();
				//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
				foreach( var message in messages ) {

					string propValue = message.XPathSelectElement( flag )?.Attribute( "value" )?.Value;
					if( propValue != excludePropValue && forumValue != "NO_POST_FIRST" ) {
						logMessageAction( $"{propValue}\t{actionContext.PackageName}\t{Path.GetFileName( path )}", null );
					}
				}
			}
		}
	}
}
