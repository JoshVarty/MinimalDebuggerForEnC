﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, InterfaceType(1), Guid("CC7BCAF4-8A68-11D2-983C-0000F808342D"), ComConversionLoss]
    public interface ICorDebugCode
    {

        void IsIL([Out] out int pbIL);

        void GetFunction([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

        void GetAddress([Out] out ulong pStart);

        void GetSize([Out] out uint pcBytes);

        void CreateBreakpoint([In] uint offset, [Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugFunctionBreakpoint ppBreakpoint);


        void GetCode([In] uint startOffset, [In] uint endOffset, [In] uint cBufferAlloc, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] buffer, [Out] out uint pcBufferSize);

        void GetVersionNumber([Out] out uint nVersion);

        void GetILToNativeMapping([In] uint cMap, [Out] out uint pcMap, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] COR_DEBUG_IL_TO_NATIVE_MAP[] map);

        void GetEnCRemapSequencePoints([In] uint cMap, [Out] out uint pcMap, [Out, MarshalAs(UnmanagedType.LPArray)] uint[] offsets);
    }

}
