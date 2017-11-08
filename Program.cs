using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blackboard.PackageAnalyzer.Actions;
using Blackboard.PackageAnalyzer.Common;

namespace Blackboard.PackageAnalyzer {

	internal class Program {

		private static readonly IDictionary<string, Action<ActionContext, Action<string, Exception>>> m_processors =
			new Dictionary<string, Action<ActionContext, Action<string, Exception>>>() {
				{ "QUIZ_GROUP_QUESTIONS_BY_TYPE", QuizActions.GetAllQuestionsGroupByType },
				{ "QUIZ_GROUP_SURVEYS_BY_TYPE", QuizActions.GetAllSurveyGroupByType },
				{ "QUIZ_NESTED_CATEGORIES", QuizActions.GetNestedCategories },
				{ "QUIZ_NESTED_CATEGORIES_WITH_ITEM", QuizActions.GetNestedCategoriesWithItem },
				{ "QUIZ_SECTION_NOT_IN_ASSESSMENT", QuizActions.GetSectionNotInAssessment },
				{ "QUIZ_OBJECTBANK", QuizActions.GetObjectbanks },
				{ "COMMON_NO_IMS_MANIFEST", CommonActions.NoImsManifest },
				{ "AVAILABLE_RESOURCE_TYPES", CommonActions.AvailableResourceTypes }
			};

		private const int DefaultConcurrencyLevel = 20;
		private const string DefaultTempDirectory = @"c:\temp\BBPackageAnalyzer";

		private static string LogFile;
		private static readonly FileSystemWrapper m_fileSystemWrapper = new FileSystemWrapper();

		private static void Main( string[] args ) {
			if( args.Length < 2 ) {
				PrintHelp();
				return;
			}

			int concurrencyLevel = args.Length >= 3 && int.TryParse( args[2], out int concurrencyLevelTmp ) ? concurrencyLevelTmp : DefaultConcurrencyLevel;
			InitLogFile();

			if( !GetPackagesDirectory( args, out string dir ) )
				return;

			if( !GetProcessor( args, out Action<ActionContext, Action<string, Exception>> processor ) )
				return;

			IEnumerable<string> paths = m_fileSystemWrapper.EnumerateDirectories( dir );
			List<Action> actions = new List<Action>();

			foreach( var path in paths.OrderBy( s => s ) ) {
				actions.Add( () => {
					string extractedPackageDirectory = path;

					ActionContext actionContext = new ActionContext( path, extractedPackageDirectory );

					Console.Write( "." );
					processor( actionContext, LogMessage );

				} );
			}

			var sw = Stopwatch.StartNew();

			var options = new ParallelOptions();
			options.MaxDegreeOfParallelism = concurrencyLevel;
			Parallel.Invoke( options, actions.ToArray() );

			Console.WriteLine();
			LogMessage( $"Elapsed time{TimeSpan.FromMilliseconds( sw.ElapsedMilliseconds )}" );
			PrintPressAnyKey();
		}

		private static bool GetProcessor( string[] args, out Action<ActionContext, Action<string, Exception>> processor ) {
			string cofigurationCode = args[1];

			if( m_processors.ContainsKey( cofigurationCode ) ) {
				processor = m_processors[cofigurationCode];
				return true;
			}

			processor = null;
			Console.WriteLine( $" Configuration with code \"{cofigurationCode}\" doesn't exists " );
			PrintPressAnyKey();
			return false;
		}

		private static void LogMessage( string message, Exception exception = null ) {
			Console.WriteLine( message );
			if( exception != null ) {
				Console.WriteLine( $"## EXCEPTION: {exception.Message}" );
				Console.WriteLine( exception );
			}

			lock( LogFile ) {
				StringBuilder fileContent = File.Exists( LogFile ) ? new StringBuilder( File.ReadAllText( LogFile ) ) : new StringBuilder();
				fileContent.AppendLine( message );
				if( exception != null ) {
					fileContent.AppendLine( $"## EXCEPTION: {exception.Message}" );
					fileContent.AppendLine( exception.ToString() );
				}
				File.WriteAllText( LogFile, fileContent.ToString() );
			}
		}

		private static void InitLogFile() {
			string workingDirectory = Path.GetDirectoryName( Assembly.GetAssembly( typeof( ActionContext ) ).Location ) ?? throw new InvalidOperationException();
			LogFile = Path.Combine( workingDirectory, "_Log.txt" );
			if( File.Exists( LogFile ) ) {
				File.Delete( LogFile );
			}
		}

		private static bool GetPackagesDirectory( string[] args, out string dir ) {
			dir = args[0];
			if( !dir.EndsWith( "\\" ) ) {
				dir += "\\";
			}

			if( !Directory.Exists( dir ) ) {
				Console.WriteLine( $" Directory \"{dir}\" doesn't exists " );
				PrintPressAnyKey();
				return false;
			}

			return true;
		}

		private static void PrintHelp() {
			Console.WriteLine( "use this syntax D2L.Blackboard.PackageAnalyzer.exe <directory> <cofiguration code> <temp dir> <skipExtraExceptions> <useUncompressedFolders> <concurrencyLevel>" );
			Console.WriteLine();
			Console.WriteLine( @"ex: D2L.Blackboard.PackageAnalyzer.exe " + @"d:\BB Packages\" + " QUIZ_NESTED_CATEGORIES 100" );
			Console.WriteLine();
			Console.WriteLine( $"Available cofiguration codes: {string.Join( ", ", m_processors.Keys )}" );
			PrintPressAnyKey();
		}

		private static void PrintPressAnyKey() {
			Console.WriteLine();
			Console.WriteLine( "press any key to continue.." );
			Console.ReadKey();
		}
	}
}
