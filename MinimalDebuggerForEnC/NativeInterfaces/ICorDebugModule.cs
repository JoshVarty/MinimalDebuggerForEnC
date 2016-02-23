﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, ComConversionLoss, InterfaceType(1), Guid("DBA2D8C1-E5C5-4069-8C13-10A7C6ABF43D")]
    public interface ICorDebugModule
    {

        void GetProcess([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);

        void GetBaseAddress([Out] out ulong pAddress);

        void GetAssembly([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugAssembly ppAssembly);

        void GetName([In] uint cchName, [Out] out uint pcchName, [MarshalAs(UnmanagedType.LPArray)] char[] szName);

        void EnableJITDebugging([In] int bTrackJITInfo, [In] int bAllowJitOpts);

        void EnableClassLoadCallbacks([In] int bClassLoadCallbacks);

        void GetFunctionFromToken([In] uint methodDef, [Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

        void GetFunctionFromRVA([In] ulong rva, [Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

        void GetClassFromToken([In] uint typeDef, [Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugClass ppClass);

        void CreateBreakpoint([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugModuleBreakpoint ppBreakpoint);

        void GetEditAndContinueSnapshot([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugEditAndContinueSnapshot ppEditAndContinueSnapshot);

        void GetMetaDataInterface([In, ComAliasName("REFIID")] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IMetadataImport ppObj);

        void GetToken([Out] out uint pToken);

        void IsDynamic([Out] out int pDynamic);

        void GetGlobalVariableValue([In] uint fieldDef, [Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

        void GetSize([Out] out uint pcBytes);

        void IsInMemory([Out] out int pInMemory);
    }

}
