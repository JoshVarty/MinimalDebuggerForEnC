using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, Guid("5263E909-8CB5-11D3-BD2F-0000F80849BD"), InterfaceType(1)]
    public interface ICorDebugUnmanagedCallback
    {
        void DebugEvent(IntPtr debugEvent, [In] int fOutOfBand);
    }
}
