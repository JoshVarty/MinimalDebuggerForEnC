using System;
using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.CorDebug.NativeApi;
using System.IO;
using System.Collections;
using System.Threading;

namespace MinimalDebuggerForEnC
{
    class Program
    {
        static void Main(string[] args)
        {
            AttachToProcess();
        }

        private static void CreateAndApplyChangesToProcess()
        {
            //Metahost class and Interface GUID
            Guid classId = new Guid("9280188D-0E8E-4867-B30C-7FA83884E8DE");
            Guid interfaceId = new Guid("D332DB9E-B9B3-4125-8207-A14884F53216");

            dynamic rawMetaHost;
            Microsoft.Samples.Debugging.CorDebug.NativeMethods.CLRCreateInstance(ref classId, ref interfaceId, out rawMetaHost);

            ICLRMetaHost metaHost = (ICLRMetaHost)rawMetaHost;

            //var runtimes = metaHost.EnumerateLoadedRuntimes(process.Handle);
            AttachToProcess();
        }

        private static void AttachToProcess()
        {
            //var process = Process.GetProcessById(processId);

            Guid classId = new Guid("9280188D-0E8E-4867-B30C-7FA83884E8DE");        //TODO: Constant with explanatory names
            Guid interfaceId = new Guid("D332DB9E-B9B3-4125-8207-A14884F53216");

            dynamic rawMetaHost;
            Microsoft.Samples.Debugging.CorDebug.NativeMethods.CLRCreateInstance(ref classId, ref interfaceId, out rawMetaHost);

            ICLRMetaHost metaHost = (ICLRMetaHost)rawMetaHost;

            var currentProcess = Process.GetCurrentProcess();
            var runtime_v40 = GetLoadedRuntimeByVersion(metaHost, currentProcess.Id, "v4.0");
            var debugger = CreateDebugger(runtime_v40.m_runtimeInfo);

            var process = Process.Start("SampleProcess.exe");

            Thread.Sleep(5000);
            var corProcess = debugger.DebugActiveProcess(process.Id, win32Attach: false);
            //var corProcess = debugger.CreateProcess("SampleProcess.exe", "", ".", 0x10);
            corProcess.OnAssemblyLoad += CorProcess_OnAssemblyLoad;
            corProcess.OnBreak += CorProcess_OnBreak1;

            var appDomains = MakeGeneric<CorAppDomain>(corProcess.AppDomains);

            var firstDomain = appDomains.First();
            var assemblies = MakeGeneric<CorAssembly>(firstDomain.Assemblies);
            var threads = MakeGeneric<CorThread>(firstDomain.Threads);

            foreach(var assembly in assemblies)
            {
                if (assembly.Name.EndsWith("SampleProcess.exe"))
                {
                    var modules = MakeGeneric<CorModule>(assembly.Modules);
                    var module = modules.First();
                    var metadataDelta = new byte[] { 66, 83, 74, 66, 1, 0, 1, 0, 0, 0, 0, 0, 12, 0, 0, 0, 118, 52, 46, 48, 46, 51, 48, 51, 49, 57, 0, 0, 0, 0, 6, 0, 124, 0, 0, 0, 96, 1, 0, 0, 35, 45, 0, 0, 220, 1, 0, 0, 148, 0, 0, 0, 35, 83, 116, 114, 105, 110, 103, 115, 0, 0, 0, 0, 112, 2, 0, 0, 24, 0, 0, 0, 35, 85, 83, 0, 136, 2, 0, 0, 48, 0, 0, 0, 35, 71, 85, 73, 68, 0, 0, 0, 184, 2, 0, 0, 28, 0, 0, 0, 35, 66, 108, 111, 98, 0, 0, 0, 212, 2, 0, 0, 0, 0, 0, 0, 35, 74, 84, 68, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 167, 1, 67, 4, 0, 192, 8, 0, 0, 0, 0, 250, 1, 51, 0, 22, 0, 0, 1, 0, 0, 0, 5, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 10, 0, 0, 0, 10, 0, 0, 0, 2, 0, 0, 0, 1, 0, 1, 2, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 14, 0, 0, 0, 87, 2, 0, 0, 42, 2, 0, 0, 18, 0, 0, 0, 49, 2, 0, 0, 66, 2, 0, 0, 14, 0, 0, 0, 203, 1, 0, 0, 42, 2, 0, 0, 14, 0, 0, 0, 250, 1, 0, 0, 42, 2, 0, 0, 14, 0, 0, 0, 232, 1, 0, 0, 42, 2, 0, 0, 4, 0, 0, 0, 0, 0, 145, 0, 218, 1, 0, 0, 153, 0, 0, 0, 0, 0, 0, 0, 121, 0, 0, 0, 80, 2, 0, 0, 133, 0, 0, 0, 129, 0, 0, 0, 240, 1, 0, 0, 139, 0, 0, 0, 3, 0, 0, 35, 0, 0, 0, 0, 4, 0, 0, 35, 0, 0, 0, 0, 13, 0, 0, 10, 0, 0, 0, 0, 14, 0, 0, 10, 0, 0, 0, 0, 12, 0, 0, 1, 0, 0, 0, 0, 13, 0, 0, 1, 0, 0, 0, 0, 14, 0, 0, 1, 0, 0, 0, 0, 15, 0, 0, 1, 0, 0, 0, 0, 16, 0, 0, 1, 0, 0, 0, 0, 2, 0, 0, 6, 0, 0, 0, 0, 12, 0, 0, 1, 13, 0, 0, 1, 14, 0, 0, 1, 15, 0, 0, 1, 16, 0, 0, 1, 2, 0, 0, 6, 13, 0, 0, 10, 14, 0, 0, 10, 3, 0, 0, 35, 4, 0, 0, 35, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 144, 0, 0, 0, 209, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 144, 0, 0, 0, 42, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 73, 110, 116, 51, 50, 0, 109, 115, 99, 111, 114, 108, 105, 98, 0, 84, 105, 109, 101, 114, 95, 69, 108, 97, 112, 115, 101, 100, 0, 67, 111, 110, 115, 111, 108, 101, 0, 87, 114, 105, 116, 101, 76, 105, 110, 101, 0, 83, 116, 114, 105, 110, 103, 0, 48, 54, 102, 54, 98, 101, 102, 52, 45, 102, 97, 99, 50, 45, 52, 102, 102, 56, 45, 98, 55, 54, 52, 45, 52, 48, 53, 97, 54, 55, 97, 54, 50, 53, 51, 98, 46, 100, 108, 108, 0, 83, 121, 115, 116, 101, 109, 0, 69, 108, 97, 112, 115, 101, 100, 69, 118, 101, 110, 116, 65, 114, 103, 115, 0, 83, 121, 115, 116, 101, 109, 46, 84, 105, 109, 101, 114, 115, 0, 67, 111, 110, 99, 97, 116, 0, 79, 98, 106, 101, 99, 116, 0, 0, 19, 49, 0, 50, 0, 51, 0, 52, 0, 53, 0, 54, 0, 55, 0, 56, 0, 32, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 67, 250, 71, 45, 35, 181, 12, 72, 156, 161, 71, 254, 150, 164, 125, 26, 108, 9, 11, 233, 54, 105, 11, 71, 187, 6, 12, 184, 183, 58, 127, 78, 0, 5, 0, 2, 14, 28, 28, 4, 0, 1, 1, 14, 8, 183, 122, 92, 86, 25, 52, 224, 137, 6, 0, 2, 1, 28, 18, 53 };
                    var ilDelta = new byte[] { 0, 0, 0, 0, 146, 0, 114, 25, 0, 0, 112, 126, 2, 0, 0, 4, 37, 23, 88, 128, 2, 0, 0, 4, 140, 14, 0, 0, 1, 40, 13, 0, 0, 10, 40, 14, 0, 0, 10, 0, 42 };
                    module.ApplyChanges(metadataDelta, ilDelta);
                }
            }

            //corProcess.Stop(-1);
        }

        private static IEnumerable<T> MakeGeneric<T>(IEnumerable enumerable)
        {
            var list = new List<T>();

            foreach(var rawItem in enumerable)
            {
                T item = (T)rawItem;
                list.Add(item);
            }

            return list;
        }

        private static void CorProcess_OnBreak1(object sender, CorThreadEventArgs e)
        {
            var appDomain = e.AppDomain;
            var assemblies = appDomain.Assemblies;
        }

        private static void CorProcess_OnAssemblyLoad(object sender, CorAssemblyEventArgs e)
        {
            var appDomain = e.AppDomain;
            var assemblies = appDomain.Assemblies;
        }

        private static void CorProcess_OnBreak(object sender, CorThreadEventArgs e)
        {
            var appDomain = e.AppDomain;
            var assemblies = appDomain.Assemblies;
        }

        private static CorDebugger CreateDebugger(ICLRRuntimeInfo runtime)
        {
            var classId = new Guid("DF8395B5-A4BA-450B-A77C-A9A47762C520");
            var interfaceId = new Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF");

            Object res = runtime.GetInterface(ref classId, ref interfaceId);
            ICorDebug debugger = (ICorDebug)res;
            //We have to initialize and set a callback in order to hook everything up
            var corDebugger = new CorDebugger(debugger);
            return corDebugger;
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

        // Retrieve information about runtimes installed on the machine (i.e. in %WINDIR%\Microsoft.NET\)
        public static IEnumerable<CLRRuntimeInfo> EnumerateInstalledRuntimes(ICLRMetaHost metaHost)
        {
            List<CLRRuntimeInfo> runtimes = new List<CLRRuntimeInfo>();
            IEnumUnknown enumRuntimes = metaHost.EnumerateInstalledRuntimes();

            // Since we're only getting one at a time, we can pass NULL for count.
            // S_OK also means we got the single element we asked for.
            for (object oIUnknown; enumRuntimes.Next(1, out oIUnknown, IntPtr.Zero) == 0; /* empty */)
            {
                runtimes.Add(new CLRRuntimeInfo(oIUnknown));
            }

            return runtimes;
        }

        // Retrieve information about runtimes that are currently loaded into the target process.
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
    }
}
