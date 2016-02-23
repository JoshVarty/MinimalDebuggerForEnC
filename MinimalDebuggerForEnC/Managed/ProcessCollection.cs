using MinimalDebuggerForEnC.Cor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Managed
{
    public class ProcessCollection : MarshalByRefObject, IEnumerable
    {
        private DebugEngine _engine;
        private List<DebugProcess> _items = new List<DebugProcess>();
        private Dictionary<CorDebugger, bool> _cleanupList = new Dictionary<CorDebugger, bool>();

        public ProcessCollection(DebugEngine engine)
        {
            _engine = engine;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public object CreateLocalProcess(CorDebugger cLRDebugger)
        {
            FreeStaleUnmanagedResources();
        }

        private void FreeStaleUnmanagedResources()
        {
            lock(_cleanupList)
            {
                var debuggersToRemove = new List<CorDebugger>();
                foreach(var clrDebuggerLookup in _cleanupList)
                {
                    if(clrDebuggerLookup.Value == true)
                    {
                        var clrDebugger = clrDebuggerLookup.Key;
                        clrDebugger.Terminate();
                        //Keep track of the debuggers we need to remove
                        debuggersToRemove.Add(clrDebugger);
                    }
                }

                //Actually remove them from the cleanup list
                foreach (var debuggerToRemove in debuggersToRemove)
                {
                    _cleanupList.Remove(debuggerToRemove);
                }
            }
        }
    }
}
