﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [Flags]
    public enum CorDebugUserState
    {
        // Fields
        USER_NONE = 0x00,
        USER_STOP_REQUESTED = 0x01,
        USER_SUSPEND_REQUESTED = 0x02,
        USER_BACKGROUND = 0x04,
        USER_UNSTARTED = 0x08,
        USER_STOPPED = 0x10,
        USER_WAIT_SLEEP_JOIN = 0x20,
        USER_SUSPENDED = 0x40,
        USER_UNSAFE_POINT = 0x80
    }
}
