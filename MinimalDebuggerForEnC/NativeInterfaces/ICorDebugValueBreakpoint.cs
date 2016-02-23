using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeInterfaces
{
    [ComImport, InterfaceType(1), Guid("CC7BCAEB-8A68-11D2-983C-0000F808342D")]
    public interface ICorDebugValueBreakpoint : ICorDebugBreakpoint
    {
        new void Activate([In] int bActive);

        new void IsActive([Out] out int pbActive);

        void GetValue([Out, MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
    }
}
