﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, ComConversionLoss, InterfaceType(1), Guid("F0E18809-72B5-11D2-976F-00A0C9B4D50C")]
    public interface ICorDebugErrorInfoEnum : ICorDebugEnum
    {
        new void Skip([In] uint celt);

        new void Reset();

        new void Clone([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugEnum ppEnum);

        new void GetCount([Out] out uint pcelt);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int Next([In] uint celt, [Out, ComAliasName("ICorDebugEditAndContinueErrorInfo**")] IntPtr errors, [Out] out uint pceltFetched);

    }
}
