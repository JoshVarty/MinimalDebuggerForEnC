using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Managed
{
    public class DebugEngine : MarshalByRefObject
    {
        private ProcessCollection _processManager;
        public DebugEngine()
        {
            _processManager = new ProcessCollection(this);
        }

        public DebugProcess Attach(int processId, string version)
        {
            return Attach(processId, null, version);
        }

        public DebugProcess Attach(int processId, SafeWin32Handle handle, string version)
        {
            //var debugProcess = _processManager.CreateLocalProcess(new CLRDebugger(version));
            return null;

        }
    }
}
