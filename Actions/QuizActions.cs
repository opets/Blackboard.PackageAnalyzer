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

			foreach( string path in resourcePaths ){
					XDocument doc = ActionHelper.GetDocument( path );
					string[] questionTypes = doc.XPathSelectElements( "//itemmetadata/bbmd_questiontype" )
						.Select( e => e?.Value )
						.Distinct()
						.OrderBy( x => x )
						.ToArray();
					//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
					foreach( string questionType in questionTypes ) logMessageAction( $"{questionType}\t{actionContext.PackageName}\t{Path.GetFileName( path )}", null );
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
				string[] questionTypes = doc.XPathSelectElements( "//itemmetadata/bbmd_questiontype" )
					.Select( e => e?.Value )
					.Distinct()
					.OrderBy( x => x )
					.ToArray();

				//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
				foreach( string questionType in questionTypes ) logMessageAction( $"{questionType}\t{actionContext.PackageName}\t{Path.GetFileName( path )}", null );
			}
		}

		public static void GetNestedCategories( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			var resourceNamesOut = new List<string>();

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				XElement[] nestedSections = doc.XPathSelectElements( "//section//section" ).ToArray();

				if( nestedSections.Any() ) resourceNamesOut.Add( Path.GetFileName( path ) );
			}

			if( resourceNamesOut.Any() ) logMessageAction( $"{actionContext.PackageName}\t {string.Join( ",", resourceNamesOut )}", null );

		}

		public static void GetNestedCategoriesWithItem( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			var resourceNamesOut = new List<string>();

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool", "assessment/x-bb-qti-survey" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths ) {

				XDocument doc = ActionHelper.GetDocument( path );
				XElement[] nestedSections = doc.XPathSelectElements( "//section//section" ).ToArray();

				if( nestedSections.Any( s => s.Elements().Any( n => n.Name != "sectionmetadata" && n.Name != "selection_ordering" ) ) ) resourceNamesOut.Add( Path.GetFileName( path ) );
			}

			if( resourceNamesOut.Any() ) logMessageAction( $"{actionContext.PackageName}\t {string.Join( ",", resourceNamesOut )}", null );
		}

		public static void GetSectionNotInAssessment( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			var resourceNamesOut = new List<string>();

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

				if( sections.Any() ) resourceNamesOut.Add( Path.GetFileName( path ) );
			}

			if( resourceNamesOut.Any() ) logMessageAction( $"{actionContext.PackageName}\t {string.Join( ",", resourceNamesOut )}", null );

		}

		public static void GetObjectbanks( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			var resourceNamesOut = new List<string>();

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool", "assessment/x-bb-qti-survey" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				XElement[] objectbanks = doc.XPathSelectElements( "//objectbank" ).ToArray();

				if( objectbanks.Any() ) resourceNamesOut.Add( Path.GetFileName( path ) );
			}

			if( resourceNamesOut.Any() ) logMessageAction( $"{actionContext.PackageName}\t {string.Join( ",", resourceNamesOut )}", null );

		}

		public static void GetAllPlainTextQuestions( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				XElement[] questionTypes = doc.XPathSelectElements( "//presentation/flow/flow[@class='QUESTION_BLOCK']/flow[@class='FORMATTED_TEXT_BLOCK']/material/mat_extension/mat_formattedtext" )
					.Where( e => "PLAIN_TEXT".Equals( e.Attribute( "type" )?.Value ) )
					.ToArray();
				//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
				foreach( XElement e in questionTypes ) logMessageAction( $"{e.Attribute( "type" )?.Value}\t{actionContext.PackageName}\t{Path.GetFileName( path )}\t{e.Value}", null );
			}
		}

		public static void GetAllFeedbackPlainTextQuestions( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				XElement[] questionTypes = doc.XPathSelectElements( "//itemfeedback//mat_extension/mat_formattedtext" )
					.Where( e => !string.Equals("HTML", e.Attribute( "type" )?.Value ) && !string.Equals( "SMART_TEXT", e.Attribute( "type" )?.Value ) )
					.ToArray();
				//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
				foreach( XElement e in questionTypes ) logMessageAction( $"{e.Attribute( "type" )?.Value}\t{actionContext.PackageName}\t{Path.GetFileName( path )}\t{e.Value}", null );
			}
		}

		public static void GetAllQuestionMattextTypes( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();

			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				XElement[] questionTypes = doc.XPathSelectElements( "//flow/flow[@class=\'QUESTION_BLOCK\']/flow[@class=\'FORMATTED_TEXT_BLOCK\']/material/mat_extension/mat_formattedtext" )
					.ToArray();
				//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
				foreach( XElement e in questionTypes.Where( e => e.Attribute( "type" )?.Value != "SMART_TEXT" ).Where( e => e.Attribute( "type" )?.Value != "HTML" ) ) {
					logMessageAction( $"{e.Attribute( "type" )?.Value}\t{actionContext.PackageName}\t{Path.GetFileName( path )}\t{e.Value}", null );
				}
			}
		}

		public static void TestQuestionWithAttribute1( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();


			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				var questions = doc.XPathSelectElements( "//item" )
					.ToArray();
				//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
				foreach( var question in questions ) {
					string questionType = question.XPathSelectElement( "itemmetadata/bbmd_questiontype" )?.Value;
					if( questionType == "Multiple Answer" && question.XPathSelectElements( "//render_choice[@shuffle='Yes']" ).Any()) {
						logMessageAction( $"{questionType}\t{actionContext.PackageName}\t{Path.GetFileName( path )}", null );
					}
				}
			}
		}

		public static void TestQuestionWithAttribute2( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();


			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				var questions = doc.XPathSelectElements( "//item" )
					.ToArray();
				//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
				foreach( var question in questions ) {
					string questionType = question.XPathSelectElement( "itemmetadata/bbmd_questiontype" )?.Value;
					if( questionType == "True/False" && 
						question.XPathSelectElements( "//flow_mat/flow_mat[@class='FORMATTED_TEXT_BLOCK']/material/mat_extension/mat_formattedtext" ).Any(e=>!string.IsNullOrEmpty( e?.Value ))) {
						logMessageAction( $"{questionType}\t{actionContext.PackageName}\t{Path.GetFileName( path )}", null );
					}
				}
			}
		}

		public static void ShortQuestionWithManyBoxes( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();


			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				var questions = doc.XPathSelectElements( "//item" )
					.ToArray();
				//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
				foreach( var question in questions ) {
					string questionType = question.XPathSelectElement( "itemmetadata/bbmd_questiontype" )?.Value;
					if( questionType == "True/False" &&
						question.XPathSelectElements( "//flow_mat/flow_mat[@class='FORMATTED_TEXT_BLOCK']/material/mat_extension/mat_formattedtext" ).Any( e => !string.IsNullOrEmpty( e?.Value ) ) ) {
						logMessageAction( $"{questionType}\t{actionContext.PackageName}\t{Path.GetFileName( path )}", null );
					}
				}
			}
		}

		public static void QuestionWithImageUri( ActionContext actionContext, Action<string, Exception> logMessageAction ) {

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();


			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				var questions = doc.XPathSelectElements( "//item" )
					.ToArray();
				//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
				foreach( var question in questions ) {
					string questionType = question.XPathSelectElement( "itemmetadata/bbmd_questiontype" )?.Value;
					string[] urls = question.XPathSelectElements( "presentation/flow/flow[@class='QUESTION_BLOCK']/flow[@class='FILE_BLOCK']/material/matapplication" )
						.Select(e=>e?.Attribute( "uri" )?.Value ).ToArray();
					if( urls.Any( e => !string.IsNullOrEmpty( e) ) ) {
						logMessageAction( $"{questionType}\t{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ",", urls )}", null );
					}
				}
			}
		}

		public static void QuestionWithProperty( ActionContext actionContext, Action<string, Exception> logMessageAction ) {
			const string propName = "bbmd_numbertype";
			const string excludePropValue = "bbmd_numbertype";

			// assessment/x-bb-qti-attempt ?
			string[] resourcePaths = actionContext
				.GetManifestResources( "assessment/x-bb-qti-test", "assessment/x-bb-qti-pool" )
				.Select( resource => Path.Combine( actionContext.ExtractedPackageDirectory, resource.FileName ) )
				.Where( File.Exists )
				.ToArray();


			foreach( string path in resourcePaths ) {
				XDocument doc = ActionHelper.GetDocument( path );
				var questions = doc.XPathSelectElements( "//item" )
					.ToArray();
				//logMessageAction( $"{actionContext.PackageName}\t{Path.GetFileName( path )}\t{string.Join( ", ", questionTypes )}", null );
				foreach( var question in questions ) {
					string propValue = question.XPathSelectElement( "itemmetadata/"+propName )?.Value;
					string questionType = question.XPathSelectElement( "itemmetadata/bbmd_questiontype" )?.Value;
					if( propValue != excludePropValue ) 
					{
						logMessageAction( $"{propValue}\t{questionType}\t{actionContext.PackageName}\t{Path.GetFileName( path )}", null );
					}
				}
			}
		}
	}
}
