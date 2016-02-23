using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, Guid("CC7BCB0B-8A68-11D2-983C-0000F808342D"), ComConversionLoss, InterfaceType(1)]
    public interface ICorDebugRegisterSet
    {

        void GetRegistersAvailable([Out] out ulong pAvailable);


        void GetRegisters([In] ulong mask, [In] uint regCount, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ulong[] regBuffer);


        void SetRegisters([In] ulong mask, [In] uint regCount, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ulong[] regBuffer);


        void GetThreadContext([In] uint contextSize, [In, ComAliasName("BYTE*")] IntPtr context);


        void SetThreadContext([In] uint contextSize, [In, ComAliasName("BYTE*")] IntPtr context);
    }

}
