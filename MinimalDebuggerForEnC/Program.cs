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
            //Metahost class and Interface GUID
            Guid classId = new Guid("9280188D-0E8E-4867-B30C-7FA83884E8DE");
            Guid interfaceId = new Guid("D332DB9E-B9B3-4125-8207-A14884F53216");

            dynamic rawMetaHost;
            Microsoft.Samples.Debugging.CorDebug.NativeMethods.CLRCreateInstance(ref classId, ref interfaceId, out rawMetaHost);

            ICLRMetaHost metaHost = (ICLRMetaHost)rawMetaHost;

            //var runtimes = metaHost.EnumerateLoadedRuntimes(process.Handle);
            AttachToProcess();

            Console.ReadLine();
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

            Thread.Sleep(2000);
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
                    //var xx = module.ApplyChanges();
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
