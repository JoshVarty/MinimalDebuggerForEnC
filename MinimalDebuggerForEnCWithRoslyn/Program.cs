using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
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
            var tree = CSharpSyntaxTree.ParseText(@"
class C
{
    public static void Main()
    {
        System.Console.WriteLine(""Hello"");
    }
}");

            var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication, moduleName: "MyCompilation");
            var compilation = CSharpCompilation.Create("MyCompilation.dll",
                syntaxTrees: new[] { tree }, references: new[] { Mscorlib }, options: options);

            var errs = compilation.GetDiagnostics().Where(n => n.Severity == DiagnosticSeverity.Error);
            if(errs.Any())
            {
                throw new Exception("Can't be any errors in your compilation");
            }

            var stream = new MemoryStream();
            var pdbStream = new MemoryStream();
            var emitResult = compilation.Emit(stream, pdbStream: pdbStream);

            //Make sure to reset the stream
            stream.Seek(0, SeekOrigin.Begin);

            var metadataModule = ModuleMetadata.CreateFromStream(stream, leaveOpen: true);

            var reader = SymReaderFactory.CreateReader(pdbStream); //TODO: Bring in native dll

            var baseline = EmitBaseline.CreateInitialBaseline(metadataModule, SymReaderFactory.CreateReader(pdbStream).GetEncMethodDebugInfo);
        }
    }
}
