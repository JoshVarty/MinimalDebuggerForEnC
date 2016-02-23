﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, InterfaceType(1), Guid("18AD3D6E-B7D2-11D2-BD04-0000F80849BD")]
    public interface ICorDebugObjectValue : ICorDebugValue
    {
        new void GetType([Out] out CorElementType pType);

        new void GetSize([Out] out uint pSize);

        new void GetAddress([Out] out ulong pAddress);

        new void CreateBreakpoint([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);

        void GetClass([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugClass ppClass);

        void GetFieldValue([In, MarshalAs(UnmanagedType.Interface)] ICorDebugClass pClass, [In] uint fieldDef, [Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

        void GetVirtualMethod([In] uint memberRef, [Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

        void GetContext([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugContext ppContext);

        void IsValueClass([Out] out int pbIsValueClass);

        void GetManagedCopy([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppObject);

        void SetFromManagedCopy([In, MarshalAs(UnmanagedType.IUnknown)] object pObject);
    }
}
