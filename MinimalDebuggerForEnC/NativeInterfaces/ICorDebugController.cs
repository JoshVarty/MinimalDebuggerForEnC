﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, Guid("3D6F5F62-7538-11D3-8D5B-00104B35E7EF"), InterfaceType(1)]
    public interface ICorDebugController
    {

        void Stop([In] uint dwTimeout);

        void Continue([In] int fIsOutOfBand);

        void IsRunning([Out] out int pbRunning);

        void HasQueuedCallbacks([In, MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread, [Out] out int pbQueued);

        void EnumerateThreads([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugThreadEnum ppThreads);

        void SetAllThreadsDebugState([In] CorDebugThreadState state, [In, MarshalAs(UnmanagedType.Interface)] ICorDebugThread pExceptThisThread);

        void Detach();

        void Terminate([In] uint exitCode);

        void CanCommitChanges([In] uint cSnapshots, [In, MarshalAs(UnmanagedType.Interface)] ref ICorDebugEditAndContinueSnapshot pSnapshots, [Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugErrorInfoEnum pError);

        void CommitChanges([In] uint cSnapshots, [In, MarshalAs(UnmanagedType.Interface)] ref ICorDebugEditAndContinueSnapshot pSnapshots, [Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugErrorInfoEnum pError);
    }
}
