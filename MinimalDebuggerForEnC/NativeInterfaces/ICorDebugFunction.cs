﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, Guid("CC7BCAF3-8A68-11D2-983C-0000F808342D"), InterfaceType(1)]
    public interface ICorDebugFunction
    {
        void GetModule([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugModule ppModule);

        void GetClass([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugClass ppClass);

        void GetToken([Out] out uint pMethodDef);

        void GetILCode([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugCode ppCode);

        void GetNativeCode([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugCode ppCode);

        void CreateBreakpoint([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugFunctionBreakpoint ppBreakpoint);

        void GetLocalVarSigToken([Out] out uint pmdSig);

        void GetCurrentVersionNumber([Out] out uint pnCurrentVersion);
    }
}
