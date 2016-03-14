using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace MinimalDebuggerForEnCWithRoslyn
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = @"
        class C
        {
            public static void Main()
            {
                System.Console.WriteLine(""Hello"");
            }
        }";
            var soln = createSolution(text);

            var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication, moduleName: "MyCompilation");
            var compilation = soln.Projects.Single().GetCompilationAsync().Result;

            var stream = new MemoryStream();
            var pdbStream = new MemoryStream();
            var emitResult = compilation.Emit(stream, pdbStream: pdbStream);
            if(!emitResult.Success)
            {
                throw new InvalidOperationException("Errors in compilation: " + emitResult.Diagnostics);
            }

            //Make sure to reset the stream
            stream.Seek(0, SeekOrigin.Begin);

            var metadataModule = ModuleMetadata.CreateFromStream(stream, leaveOpen: true);

            var reader = SymReaderFactory.CreateReader(pdbStream); 

            var baseline = EmitBaseline.CreateInitialBaseline(metadataModule, SymReaderFactory.CreateReader(pdbStream).GetEncMethodDebugInfo);

            dynamic csharpEditAndContinueAnalyzer = Activator.CreateInstance("Microsoft.CodeAnalysis.CSharp.Features", "Microsoft.CodeAnalysis.CSharp.EditAndContinue.CSharpEditAndContinueAnalyzer");


            //public Task<DocumentAnalysisResults> AnalyzeDocumentAsync(
            //    Solution baseSolution, 
            //    ImmutableArray<ActiveStatementSpan> 
            //    baseActiveStatements, 
            //    Document document, 
            //    CancellationToken cancellationToken);

            //dynamic results = csharpEditAndContinueAnalyzer.AnalyzeDocumentAsync(soln, ImmutableArray<ActiveStatementSpan>.Create())
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
