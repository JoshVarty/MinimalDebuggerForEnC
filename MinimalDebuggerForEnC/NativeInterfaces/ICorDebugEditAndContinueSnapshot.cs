﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IStream = System.Runtime.InteropServices.ComTypes.IStream;

namespace MinimalDebuggerForEnC.NativeInterfaces
{

    [ComImport, InterfaceType(1), Guid("6DC3FA01-D7CB-11D2-8A95-0080C792E5D8")]
    public interface ICorDebugEditAndContinueSnapshot
    {
        void CopyMetaData([In, MarshalAs(UnmanagedType.Interface)] IStream pIStream, [Out] out Guid pMvid);

        void GetMvid([Out] out Guid pMvid);

        void GetRoDataRVA([Out] out uint pRoDataRVA);

        void GetRwDataRVA([Out] out uint pRwDataRVA);

        void SetPEBytes([In, MarshalAs(UnmanagedType.Interface)] IStream pIStream);

        void SetILMap([In] uint mdFunction, [In] uint cMapSize, [In] ref COR_IL_MAP map);

        void SetPESymbolBytes([In, MarshalAs(UnmanagedType.Interface)] IStream pIStream);
    }

}
