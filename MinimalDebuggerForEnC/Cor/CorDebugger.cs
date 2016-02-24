using MinimalDebuggerForEnC.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Cor
{
    public sealed class CorDebugger
    {
        private readonly ICorDebug codebugger;

        /// <summary>
        /// Creates a new ICorDebug instance, initializes it
        /// and sets the managed callback listener.
        /// </summary>
        /// <param name="codebugger">ICorDebug COM object.</param>
        internal CorDebugger(ICorDebug codebugger)
        {
            this.codebugger = codebugger;

            this.codebugger.Initialize();
            this.codebugger.SetManagedHandler(new ManagedCallback());
        }

        /// <summary>
        /// Creates a new managed process. 
        /// </summary>
        /// <param name="exepath">application executable file</param>
        /// <param name="currdir">starting directory</param>
        /// <returns></returns>
        public CorProcess CreateProcess(String exepath, String currdir = ".")
        {
            STARTUPINFO si = new STARTUPINFO();
            si.cb = Marshal.SizeOf(si);

            // initialize safe handles 
            si.hStdInput = new Microsoft.Win32.SafeHandles.SafeFileHandle(new IntPtr(0), false);
            si.hStdOutput = new Microsoft.Win32.SafeHandles.SafeFileHandle(new IntPtr(0), false);
            si.hStdError = new Microsoft.Win32.SafeHandles.SafeFileHandle(new IntPtr(0), false);

            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            ICorDebugProcess proc;
            codebugger.CreateProcess(
                                exepath,
                                exepath,
                                null,
                                null,
                                1, // inherit handles
                                (UInt32)CreateProcessFlags.CREATE_NEW_CONSOLE,
                                new IntPtr(0),
                                ".",
                                si,
                                pi,
                                CorDebugCreateProcessFlags.DEBUG_NO_SPECIAL_OPTIONS,
                                out proc);
            // FIXME close handles (why?)

            return new CorProcess(proc);
        }

        /// <summary>
        /// Attaches to the process with the given pid.
        /// </summary>
        /// <param name="pid">active process id</param>
        /// <param name="win32Attach"></param>
        /// <returns></returns>
        public CorProcess DebugActiveProcess(Int32 pid, Boolean win32Attach = false)
        {
            ICorDebugProcess coproc;
            codebugger.DebugActiveProcess(Convert.ToUInt32(pid), win32Attach ? 1 : 0, out coproc);

            return new CorProcess(coproc);
        }

        #region ICorDebugManagedCallback events

        private class ManagedCallback : ICorDebugManagedCallback, ICorDebugManagedCallback2
        {
            void HandleEvent(ICorDebugController controller)
            {
                Console.WriteLine("test event");
                controller.Continue(0);
            }

            void ICorDebugManagedCallback.Breakpoint(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.StepComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugStepper pStepper, CorDebugStepReason reason)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.Break(ICorDebugAppDomain pAppDomain, ICorDebugThread thread)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int unhandled)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.EvalComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval pEval)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.EvalException(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval pEval)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.CreateProcess(ICorDebugProcess pProcess)
            {
                HandleEvent(pProcess);
            }

            void ICorDebugManagedCallback.ExitProcess(ICorDebugProcess pProcess)
            {
            }

            void ICorDebugManagedCallback.CreateThread(ICorDebugAppDomain pAppDomain, ICorDebugThread thread)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.ExitThread(ICorDebugAppDomain pAppDomain, ICorDebugThread thread)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.LoadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.UnloadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.LoadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.UnloadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.DebuggerError(ICorDebugProcess pProcess, int errorHR, uint errorCode)
            {
                HandleEvent(pProcess);
            }

            void ICorDebugManagedCallback.LogMessage(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, string pLogSwitchName, string pMessage)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.LogSwitch(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, uint ulReason, string pLogSwitchName, string pParentName)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.CreateAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
            {
                pAppDomain.Attach();
                HandleEvent(pProcess);
            }

            void ICorDebugManagedCallback.ExitAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
            {
                HandleEvent(pProcess);
            }

            void ICorDebugManagedCallback.LoadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.UnloadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.ControlCTrap(ICorDebugProcess pProcess)
            {
                HandleEvent(pProcess);
            }

            void ICorDebugManagedCallback.NameChange(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.UpdateModuleSymbols(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule, System.Runtime.InteropServices.ComTypes.IStream pSymbolStream)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.EditAndContinueRemap(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction, int fAccurate)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback.BreakpointSetError(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint, uint dwError)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback2.FunctionRemapOpportunity(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pOldFunction, ICorDebugFunction pNewFunction, uint oldILOffset)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback2.CreateConnection(ICorDebugProcess pProcess, uint dwConnectionId, ref ushort pConnName)
            {
                HandleEvent(pProcess);
            }

            void ICorDebugManagedCallback2.ChangeConnection(ICorDebugProcess pProcess, uint dwConnectionId)
            {
                HandleEvent(pProcess);
            }

            void ICorDebugManagedCallback2.DestroyConnection(ICorDebugProcess pProcess, uint dwConnectionId)
            {
                HandleEvent(pProcess);
            }

            void ICorDebugManagedCallback2.Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFrame pFrame, uint nOffset, CorDebugExceptionCallbackType dwEventType, uint dwFlags)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback2.ExceptionUnwind(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback2.FunctionRemapComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction)
            {
                HandleEvent(pAppDomain);
            }

            void ICorDebugManagedCallback2.MDANotification(ICorDebugController pController, ICorDebugThread pThread, ICorDebugMDA pMDA)
            {
                HandleEvent(pController);
            }
        }

        #endregion

    }
}
