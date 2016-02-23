﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, ComConversionLoss, Guid("DF59507C-D47A-459E-BCE2-6427EAC8FD06"), InterfaceType(1)]
    public interface ICorDebugAssembly
    {
        void GetProcess([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);

        void GetAppDomain([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugAppDomain ppAppDomain);

        void EnumerateModules([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugModuleEnum ppModules);

        void GetCodeBase([In] uint cchName, [Out] out uint pcchName, [MarshalAs(UnmanagedType.LPArray)] char[] szName);

        void GetName([In] uint cchName, [Out] out uint pcchName, [MarshalAs(UnmanagedType.LPArray)] char[] szName);
    }

}
