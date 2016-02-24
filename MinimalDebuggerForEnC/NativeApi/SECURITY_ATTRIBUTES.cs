using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.NativeApi
{
    [StructLayout(LayoutKind.Sequential, Pack = 8), ComVisible(false)]
    public class SECURITY_ATTRIBUTES
    {
        public int nLength;
        private IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
        public SECURITY_ATTRIBUTES() { }
    }
}
