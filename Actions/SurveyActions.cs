using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using Blackboard.PackageAnalyzer.Common;

namespace Blackboard.PackageAnalyzer.Actions {
	internal static class SurveyActions {
		public static void GetNestedAssessment( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			var resources = actionContext
				.GetManifestResources( "*" )
				.ToArray();

			foreach( ResourceElement resource in resources ) {
				try {
					string path = Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName );

					XDocument doc = ActionHelper.GetDocument( path );
					XElement[] nestedSections = doc.XPathSelectElements( "//questestinterop//assessment[@title='Survey']/assessment" ).ToArray();

					if( nestedSections .Any()) {
						logMessageAction( $"{actionContext.PackageName}\t${ resource.ResourceType}\t${ resource.FileName}\t", null );
					}
				} catch(Exception ex) {
					
				}
			}
		}
	}
}
