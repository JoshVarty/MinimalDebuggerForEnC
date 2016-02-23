﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, Guid("CC7BCAEA-8A68-11D2-983C-0000F808342D"), InterfaceType(1)]
    public interface ICorDebugModuleBreakpoint : ICorDebugBreakpoint
    {
        new void Activate([In] int bActive);

        new void IsActive([Out] out int pbActive);


        void GetModule([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugModule ppModule);
    }
}
