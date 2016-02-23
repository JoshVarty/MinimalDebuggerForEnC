﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, Guid("938C6D66-7FB6-4F69-B389-425B8987329B"), InterfaceType(1)]
    public interface ICorDebugThread
    {

        void GetProcess([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);

        void GetID([Out] out uint pdwThreadId);

        void GetHandle([Out] out IntPtr phThreadHandle);

        void GetAppDomain([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugAppDomain ppAppDomain);

        void SetDebugState([In] CorDebugThreadState state);

        void GetDebugState([Out] out CorDebugThreadState pState);

        void GetUserState([Out] out CorDebugUserState pState);

        void GetCurrentException([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppExceptionObject);

        void ClearCurrentException();

        void CreateStepper([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugStepper ppStepper);

        void EnumerateChains([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugChainEnum ppChains);

        void GetActiveChain([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

        void GetActiveFrame([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);

        void GetRegisterSet([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugRegisterSet ppRegisters);

        void CreateEval([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugEval ppEval);

        void GetObject([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppObject);
    }

}
