using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnCWithRoslyn
{
    class Program
    {
        static readonly Type _csharpEditAndContinueAnalyzerType = Type.GetType("Microsoft.CodeAnalysis.CSharp.EditAndContinue.CSharpEditAndContinueAnalyzer, Microsoft.CodeAnalysis.CSharp.Features");
        static readonly Type _activeStatementSpanType = Type.GetType("Microsoft.CodeAnalysis.EditAndContinue.ActiveStatementSpan, Microsoft.CodeAnalysis.Features");

        static void Main(string[] args)
        {
            Go().Wait();
        }

        private static IEnumerable<SemanticEdit> GetEdits(Solution originalSolution, Document newDocument, CancellationToken token = default(CancellationToken))
        {
            dynamic csharpEditAndContinueAnalyzer = Activator.CreateInstance(_csharpEditAndContinueAnalyzerType, nonPublic: true);

            var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
            Type[] targetParams = new Type[] { };

            var immutableArray_Create_T = typeof(ImmutableArray).GetMethod("Create", bindingFlags, binder: null, types: targetParams, modifiers: null);
            var immutableArray_Create_ActiveStatementSpan = immutableArray_Create_T.MakeGenericMethod(_activeStatementSpanType);

            var immutableArray_ActiveStatementSpan = immutableArray_Create_ActiveStatementSpan.Invoke(null, new object[] { });
            var method = (MethodInfo)csharpEditAndContinueAnalyzer.GetType().GetMethod("AnalyzeDocumentAsync");
            var myParams = new object[] { originalSolution, immutableArray_ActiveStatementSpan, newDocument, token };
            object task = method.Invoke(csharpEditAndContinueAnalyzer, myParams);

            var documentAnalysisResults = task.GetType().GetProperty("Result").GetValue(task);

            var edits = (IEnumerable<SemanticEdit>)documentAnalysisResults.GetType().GetField("SemanticEdits", bindingFlags).GetValue(documentAnalysisResults);
            return edits;
        }

        private static async Task Go()
        {
            var text = @"
        class C
        {
            public static void Main()
            {
                int x = 5;
                System.Console.WriteLine(5);
            }
        }";

            var newText = @"
        class C
        {
            public static void Main()
            {
                int x = 5 + 10;
                System.Console.WriteLine(x);
            }
        }";
            var soln = createSolution(text);

            var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication, moduleName: "MyCompilation");
            var compilation = soln.Projects.Single().GetCompilationAsync().Result;

            var stream = new MemoryStream();
            var pdbStream = new MemoryStream();
            var emitResult = compilation.Emit(stream, pdbStream: pdbStream);
            if (!emitResult.Success)
            {
                throw new InvalidOperationException("Errors in compilation: " + emitResult.Diagnostics);
            }

            //Make sure to reset the stream
            stream.Seek(0, SeekOrigin.Begin);

            var metadataModule = ModuleMetadata.CreateFromStream(stream, leaveOpen: true);
            var reader = SymReaderFactory.CreateReader(pdbStream);
            var baseline = EmitBaseline.CreateInitialBaseline(metadataModule, SymReaderFactory.CreateReader(pdbStream).GetEncMethodDebugInfo);

            //TODO: Generate a document with differences.
            var document = soln.Projects.Single().Documents.Single();
            var newDocument = document.WithText(SourceText.From(newText, System.Text.ASCIIEncoding.ASCII));

            var edits = GetEdits(soln, newDocument);

            var metadataStream = new MemoryStream();
            var ilStream = new MemoryStream();
            var newPdbStream = new MemoryStream();
            var updatedMethods = new List<System.Reflection.Metadata.MethodDefinitionHandle>();

            var newEmitResult = compilation.EmitDifference(baseline, edits, metadataStream, ilStream, newPdbStream, updatedMethods);

            var buf = ilStream.GetBuffer();
        }

        private static Solution createSolution(string text)
        {
            var tree = CSharpSyntaxTree.ParseText(text);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var adHockWorkspace = new AdhocWorkspace();

            var project = adHockWorkspace.AddProject(ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Default, "MyProject", "MyProject", "C#", metadataReferences: new List<MetadataReference>() { mscorlib } ));

            adHockWorkspace.AddDocument(project.Id, "MyDocument.cs", SourceText.From(text, System.Text.Encoding.ASCII));

            return adHockWorkspace.CurrentSolution;
        }
    }
}
