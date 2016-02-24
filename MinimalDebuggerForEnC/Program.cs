using Microsoft.Win32.SafeHandles;
using MinimalDebuggerForEnC.Cor;
using MinimalDebuggerForEnC.NativeApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC
{
    class Program
    {
        static void Main(string[] args)
        {
            int processId = 8016;
            AttachToProcess(processId);
            Console.ReadLine();
        }

        private static void AttachToProcess(int processId)
        {
            var process = Process.GetProcessById(processId);

            Guid classId = new Guid("9280188D-0E8E-4867-B30C-7FA83884E8DE");        //TODO: Constant with explanatory names
            Guid interfaceId = new Guid("D332DB9E-B9B3-4125-8207-A14884F53216");

            var metaHost = NativeMethods.CLRCreateInstance(ref classId, ref interfaceId);

            //Get the runtimes available in our target process
            var runtimes = metaHost.EnumerateLoadedRuntimes(process.Handle);
            var runtime = GetRuntime(runtimes, "v4.0");

            var debugger = CreateDebugger(runtime);

            debugger.DebugActiveProcess(processId);

        }

        private static CorDebugger CreateDebugger(ICLRRuntimeInfo runtime)
        {
            var classId = new Guid("DF8395B5-A4BA-450B-A77C-A9A47762C520");
            var interfaceId = new Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF");

            Object res;
            runtime.GetInterface(ref classId, ref interfaceId, out res);
            ICorDebug debugger = (ICorDebug)res;
            //We have to initialize and set a callback in order to hook everything up
            var corDebugger = new CorDebugger(debugger);
            return corDebugger;
        }

        private static ICLRRuntimeInfo GetRuntime(IEnumUnknown runtimes, String version)
        {
            Object[] temparr = new Object[3];
            UInt32 fetchedNum;
            do
            {
                runtimes.Next(Convert.ToUInt32(temparr.Length), temparr, out fetchedNum);

                for (Int32 i = 0; i < fetchedNum; i++)
                {
                    ICLRRuntimeInfo t = (ICLRRuntimeInfo)temparr[i];

                    // version not specified we return the first one
                    if (String.IsNullOrEmpty(version))
                    {
                        return t;
                    }

                    // initialize buffer for the runtime version string
                    StringBuilder sb = new StringBuilder(16);
                    UInt32 len = Convert.ToUInt32(sb.Capacity);
                    t.GetVersionString(sb, ref len);
                    if (sb.ToString().StartsWith(version, StringComparison.Ordinal))
                    {
                        return t;
                    }
                }
            } while (fetchedNum == temparr.Length);

            return null;
        }
    }
}
