﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{

    [ComImport, Guid("CC7BCAEC-8A68-11D2-983C-0000F808342D"), InterfaceType(1)]
    public interface ICorDebugStepper
    {
        void IsActive([Out] out int pbActive);

        void Deactivate();

        void SetInterceptMask([In] CorDebugIntercept mask);

        void SetUnmappedStopMask([In] CorDebugUnmappedStop mask);

        void Step([In] int bStepIn);

        void StepRange([In] int bStepIn, [In, MarshalAs(UnmanagedType.LPArray)] COR_DEBUG_STEP_RANGE[] ranges, [In] uint cRangeCount);

        void StepOut();

        void SetRangeIL([In] int bIL);
    }

}
