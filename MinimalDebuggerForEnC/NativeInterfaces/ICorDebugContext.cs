﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, InterfaceType(1), Guid("CC7BCB00-8A68-11D2-983C-0000F808342D")]
    public interface ICorDebugContext : ICorDebugObjectValue
    {
        new void GetType([Out] out CorElementType pType);

        new void GetSize([Out] out uint pSize);

        new void GetAddress([Out] out ulong pAddress);

        new void CreateBreakpoint([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);


        new void GetClass([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugClass ppClass);

        new void GetFieldValue([In, MarshalAs(UnmanagedType.Interface)] ICorDebugClass pClass, [In] uint fieldDef, [Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

        new void GetVirtualMethod([In] uint memberRef, [Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

        new void GetContext([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugContext ppContext);

        new void IsValueClass([Out] out int pbIsValueClass);

        new void GetManagedCopy([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppObject);

        new void SetFromManagedCopy([In, MarshalAs(UnmanagedType.IUnknown)] object pObject);
    }
}
