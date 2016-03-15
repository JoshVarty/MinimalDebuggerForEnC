using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.CorDebug.NativeApi;
using Microsoft.Samples.Debugging.CorMetadata.NativeApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
            string sourceText_1 = @"
using System;
using System.Threading.Tasks;
 
class C
{
    public static void F() { Console.WriteLine(123); }
    public static void Main() { Console.WriteLine(1); Console.ReadLine(); F(); Console.ReadLine(); }
}";

            string sourceText_2 = @"
using System;
using System.Threading.Tasks;
 
class C 
{ 
    public static void F() { Console.WriteLine(123123); }
    public static void Main() { Console.WriteLine(1); Console.ReadLine(); F(); Console.ReadLine(); }
}";

            string programName = "MyProgram.exe";
            string pdbName = "MyProgram.pdb";

            //Get solution 
            var solution = createSolution(sourceText_1);

            //Compile code and emit to disk
            var baseline = EmitAndGetBaseLine(solution, programName, pdbName);

            //Get Debugger
            var debugger = GetDebugger();

            //Start process with debugger
            var corProcess = StartProcess(debugger, programName);

            //Wait for it to start up and for modules to get changed
            Console.WriteLine("Sleeping for one second while it loads...");
            Thread.Sleep(1000);

            //Take solution, change it
            var document = solution.Projects.Single().Documents.Single();
            var updatedDocument = document.WithText(SourceText.From(sourceText_2, System.Text.Encoding.UTF8));

            //Get new compilation
            var newCompilation = updatedDocument.Project.GetCompilationAsync().Result;

            //Get edits 
            var semanticEdits = GetEdits(solution, updatedDocument);

            //Apply metadat/IL deltas
            var metadataStream = new MemoryStream();
            var ilStream = new MemoryStream();
            var newPdbStream = new MemoryStream();
            var updatedMethods = new List<System.Reflection.Metadata.MethodDefinitionHandle>();
            var newEmitResult = newCompilation.EmitDifference(baseline, semanticEdits, metadataStream, ilStream, newPdbStream, updatedMethods);

            //Apply differences
            var appDomain = corProcess.AppDomains.Cast<CorAppDomain>().Single();
            var assembly = appDomain.Assemblies.Cast<CorAssembly>().Where(n => n.Name.Contains("MyProgram")).Single();
            var module = assembly.Modules.Cast<CorModule>().Single();

            //I found a bug in the ICorDebug API. Apparently the API assumes that you couldn't possibly have a change to apply
            //unless you had first fetched the metadata for this module. Perhaps reasonable in the overall scenario, but
            //its certainly not OK to simply throw an AV if it hadn't happened yet.
            //
            //In any case, fetching the metadata is a thankfully simple workaround
            object import = module.GetMetaDataInterface(typeof(IMetadataImport).GUID);
            corProcess.Stop(-1);
            module.ApplyChanges(metadataStream.ToArray(), ilStream.ToArray());
            corProcess.Continue(outOfBand: false);
            
            //Does code change?
        }

        private static CorProcess StartProcess(CorDebugger debugger, string programName)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var corProcess = debugger.CreateProcess(programName, "", currentDirectory, 0x10);
            corProcess.OnModuleLoad += CorProcess_OnModuleLoad;
            corProcess.Continue(outOfBand: false);

            return corProcess;
        }

        private static void CorProcess_OnModuleLoad(object sender, CorModuleEventArgs e)
        {
            var module = e.Module;
            if (!module.Name.Contains("MyProgram"))
            {
                return;
            }

            Console.Write("Is running: " + e.Process.IsRunning());
            var compilerFlags = module.JITCompilerFlags;
            module.JITCompilerFlags = CorDebugJITCompilerFlags.CORDEBUG_JIT_ENABLE_ENC;
        }

        private static CorDebugger GetDebugger()
        {
            Guid classId = new Guid("9280188D-0E8E-4867-B30C-7FA83884E8DE");        //TODO: Constant with explanatory names
            Guid interfaceId = new Guid("D332DB9E-B9B3-4125-8207-A14884F53216");

            dynamic rawMetaHost;
            Microsoft.Samples.Debugging.CorDebug.NativeMethods.CLRCreateInstance(ref classId, ref interfaceId, out rawMetaHost);
            ICLRMetaHost metaHost = (ICLRMetaHost)rawMetaHost;

            var currentProcess = Process.GetCurrentProcess();
            var runtime_v40 = GetLoadedRuntimeByVersion(metaHost, currentProcess.Id, "v4.0");

            var debuggerClassId = new Guid("DF8395B5-A4BA-450B-A77C-A9A47762C520");
            var debuggerInterfaceId = new Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF");

            Object res = runtime_v40.m_runtimeInfo.GetInterface(ref debuggerClassId, ref debuggerInterfaceId);
            ICorDebug debugger = (ICorDebug)res;
            //We have to initialize and set a callback in order to hook everything up
            var corDebugger = new CorDebugger(debugger);
            return corDebugger;
        }

        private static EmitBaseline EmitAndGetBaseLine(Solution solution, string programName, string pdbName)
        {
            var compilation = solution.Projects.Single().GetCompilationAsync().Result;

            var emitResult = compilation.Emit(programName, pdbName);
            if(!emitResult.Success)
            {
                throw new InvalidOperationException("Errors in compilation: " + emitResult.Diagnostics.Count());
            }

            var metadataModule = ModuleMetadata.CreateFromFile(programName);
            var fs = new FileStream(pdbName, FileMode.Open);
            var baseline = EmitBaseline.CreateInitialBaseline(metadataModule, SymReaderFactory.CreateReader(fs).GetEncMethodDebugInfo);
            return baseline;
        }

        public static CLRRuntimeInfo GetLoadedRuntimeByVersion(ICLRMetaHost metaHost, Int32 processId, string version)
        {
            IEnumerable<CLRRuntimeInfo> runtimes = EnumerateLoadedRuntimes(metaHost, processId);

            foreach (CLRRuntimeInfo rti in runtimes)
            {
                if (rti.GetVersionString().StartsWith(version, StringComparison.OrdinalIgnoreCase))
                {
                    return rti;
                }
            }

            return null;
        }

        public static IEnumerable<CLRRuntimeInfo> EnumerateLoadedRuntimes(ICLRMetaHost metaHost, Int32 processId)
        {
            List<CLRRuntimeInfo> runtimes = new List<CLRRuntimeInfo>();
            IEnumUnknown enumRuntimes;

            using (ProcessSafeHandle hProcess = NativeMethods.OpenProcess((int)(NativeMethods.ProcessAccessOptions.ProcessVMRead |
                                                                        NativeMethods.ProcessAccessOptions.ProcessQueryInformation |
                                                                        NativeMethods.ProcessAccessOptions.ProcessDupHandle |
                                                                        NativeMethods.ProcessAccessOptions.Synchronize),
                                                                        false, // inherit handle
                                                                        processId))
            {
                if (hProcess.IsInvalid)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }

                enumRuntimes = metaHost.EnumerateLoadedRuntimes(hProcess);
            }

            // Since we're only getting one at a time, we can pass NULL for count.
            // S_OK also means we got the single element we asked for.
            for (object oIUnknown; enumRuntimes.Next(1, out oIUnknown, IntPtr.Zero) == 0; /* empty */)
            {
                runtimes.Add(new CLRRuntimeInfo(oIUnknown));
            }

            return runtimes;
        }

        private static IEnumerable<SemanticEdit> GetEdits(Solution originalSolution, Document updatedDocument, CancellationToken token = default(CancellationToken))
        {
            dynamic csharpEditAndContinueAnalyzer = Activator.CreateInstance(_csharpEditAndContinueAnalyzerType, nonPublic: true);

            var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
            Type[] targetParams = new Type[] { };

            var immutableArray_Create_T = typeof(ImmutableArray).GetMethod("Create", bindingFlags, binder: null, types: targetParams, modifiers: null);
            var immutableArray_Create_ActiveStatementSpan = immutableArray_Create_T.MakeGenericMethod(_activeStatementSpanType);

            var immutableArray_ActiveStatementSpan = immutableArray_Create_ActiveStatementSpan.Invoke(null, new object[] { });
            var method = (MethodInfo)csharpEditAndContinueAnalyzer.GetType().GetMethod("AnalyzeDocumentAsync");
            var myParams = new object[] { originalSolution, immutableArray_ActiveStatementSpan, updatedDocument, token };
            object task = method.Invoke(csharpEditAndContinueAnalyzer, myParams);

            var documentAnalysisResults = task.GetType().GetProperty("Result").GetValue(task);

            var edits = (IEnumerable<SemanticEdit>)documentAnalysisResults.GetType().GetField("SemanticEdits", bindingFlags).GetValue(documentAnalysisResults);
            return edits;
        }

        private static Solution createSolution(string text)
        {
            var tree = CSharpSyntaxTree.ParseText(text);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var adHockWorkspace = new AdhocWorkspace();

            var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication, platform: Platform.X86);
            var project = adHockWorkspace.AddProject(ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Default, "MyProject", "MyProject", "C#", metadataReferences: new List<MetadataReference>() { mscorlib }, compilationOptions: options ));

            adHockWorkspace.AddDocument(project.Id, "MyDocument.cs", SourceText.From(text, System.Text.UTF8Encoding.UTF8));
            return adHockWorkspace.CurrentSolution;
        }
    }
}
