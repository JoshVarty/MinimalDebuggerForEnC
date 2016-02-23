﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, ComConversionLoss, InterfaceType(1), Guid("CC7BCB05-8A68-11D2-983C-0000F808342D")]
    public interface ICorDebugProcessEnum : ICorDebugEnum
    {
        new void Skip([In] uint celt);

        new void Reset();

        new void Clone([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugEnum ppEnum);

        new void GetCount([Out] out uint pcelt);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int Next([In] uint celt, [Out, MarshalAs(UnmanagedType.LPArray)] ICorDebugProcess[] processes, [Out] out uint pceltFetched);
    }
}
