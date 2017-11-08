using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Blackboard.PackageAnalyzer.Actions {
	internal static class QuizActions {

		public static void GetAllQuestionsGroupByType( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths )
				if( File.ReadAllText( path ).Contains( "qticomment" ) ) {
					XDocument doc = ActionHelper.GetDocument( path );
					var questionTypes = doc.XPathSelectElements( "//itemmetadata/bbmd_questiontype" )
						.Select( e => e?.Value )
						.Distinct()
						.OrderBy( x => x )
						.ToArray();
					//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
					foreach( string questionType in questionTypes ) {
						logMessageAction( $"{questionType}\t{actionContext.PackageName}\t{Path.GetFileName( path )}", null );
					}
				}
		}

		public static void GetAllSurveyGroupByType( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-survey" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				var questionTypes = doc.XPathSelectElements( "//itemmetadata/bbmd_questiontype" )
					.Select( e => e?.Value )
					.Distinct()
					.OrderBy( x => x )
					.ToArray();

				//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
				foreach( string questionType in questionTypes ) {
					logMessageAction( $"{questionType}\t{actionContext.PackageName}\t{Path.GetFileName( path )}", null );
				}
			}
		}

		public static void GetNestedCategories( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			List<string> resourceNamesOut = new List<string>();

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				XElement[] nestedSections = doc.XPathSelectElements( "//section//section" ).ToArray();

				if( nestedSections.Any() ) {
					resourceNamesOut.Add( Path.GetFileName( path ) );
				}
			}

			if( resourceNamesOut.Any() ) {
				logMessageAction( $"{actionContext.PackageName}\t {string.Join( ",", resourceNamesOut )}", null );
			}

		}

		public static void GetNestedCategoriesWithItem( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			List<string> resourceNamesOut = new List<string>();

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool", "assessment/x-bb-qti-survey" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths ) {

				XDocument doc = ActionHelper.GetDocument( path );
				XElement[] nestedSections = doc.XPathSelectElements( "//section//section" ).ToArray();

				if( nestedSections.Any( s => s.Elements().Any( n => n.Name != "sectionmetadata" && n.Name != "selection_ordering" ) ) ) {
					resourceNamesOut.Add( Path.GetFileName( path ) );
				}
			}

			if( resourceNamesOut.Any() ) {
				logMessageAction( $"{actionContext.PackageName}\t {string.Join( ",", resourceNamesOut )}", null );
			}
		}
		
		public static void GetSectionNotInAssessment( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			List<string> resourceNamesOut = new List<string>();

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool", "assessment/x-bb-qti-survey" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths ) {

				XDocument doc = ActionHelper.GetDocument( path );
				string[] sections = doc.XPathSelectElements( "//section" )
					.Select( e => e.GetPath() )
					.Where( p => !p.Contains( "assessment" ) )
					.ToArray();

				if( sections.Any() ) {
					resourceNamesOut.Add( Path.GetFileName( path ) );
				}
			}

			if( resourceNamesOut.Any() ) {
				logMessageAction( $"{actionContext.PackageName}\t {string.Join( ",", resourceNamesOut )}", null );
			}

		}

		public static void GetObjectbanks( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			List<string> resourceNamesOut = new List<string>();

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool", "assessment/x-bb-qti-survey" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				XElement[] objectbanks = doc.XPathSelectElements( "//objectbank" ).ToArray();

				if( objectbanks.Any() ) {
					resourceNamesOut.Add( Path.GetFileName( path ) );
				}
			}

			if( resourceNamesOut.Any() ) {
				logMessageAction( $"{actionContext.PackageName}\t {string.Join( ",", resourceNamesOut )}", null );
			}

		}


	}
}
