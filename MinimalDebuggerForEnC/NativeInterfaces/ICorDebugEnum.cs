﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, InterfaceType(1), Guid("CC7BCB01-8A68-11D2-983C-0000F808342D")]
    public interface ICorDebugEnum
    {

        void Skip([In] uint celt);

        void Reset();

        void Clone([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugEnum ppEnum);

        void GetCount([Out] out uint pcelt);
    }
}
