using MinimalDebuggerForEnC.NativeInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Cor
{
    public class CorDebugger : MarshalByRefObject
    {
        private const int MaxVersionStringLength = 256;
        private ICorDebug _debugger = null;

        public void Terminate()
        {
        }
    }
}
