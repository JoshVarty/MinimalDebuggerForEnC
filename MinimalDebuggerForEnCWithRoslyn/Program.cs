using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

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

            var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);

            stream.Seek(0, SeekOrigin.Begin);
            var metadataModule = ModuleMetadata.CreateFromStream(stream, leaveOpen: true);
        }
    }
}
