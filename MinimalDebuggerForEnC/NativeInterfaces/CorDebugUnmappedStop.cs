﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [Flags]
    public enum CorDebugUnmappedStop
    {
        // Fields
        STOP_ALL = 0xffff,
        STOP_NONE = 0,
        STOP_PROLOG = 1,
        STOP_EPILOG = 2,
        STOP_NO_MAPPING_INFO = 4,
        STOP_OTHER_UNMAPPED = 8,
        STOP_UNMANAGED = 0x10
    }
}
