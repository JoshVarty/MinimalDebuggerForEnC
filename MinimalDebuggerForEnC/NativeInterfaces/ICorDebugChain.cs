﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, Guid("CC7BCAEE-8A68-11D2-983C-0000F808342D"), InterfaceType(1)]
    public interface ICorDebugChain
    {
        void GetThread([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugThread ppThread);

        void GetStackRange([Out] out ulong pStart, [Out] out ulong pEnd);

        void GetContext([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugContext ppContext);

        void GetCaller([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

        void GetCallee([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

        void GetPrevious([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

        void GetNext([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

        void IsManaged([Out] out int pManaged);

        void EnumerateFrames([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugFrameEnum ppFrames);

        void GetActiveFrame([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);

        void GetRegisterSet([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugRegisterSet ppRegisters);

        void GetReason([Out] out CorDebugChainReason pReason);
    }
}
