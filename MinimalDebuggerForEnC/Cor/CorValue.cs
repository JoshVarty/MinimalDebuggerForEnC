using MinimalDebuggerForEnC.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Cor
{
    public class CorValue : WrapperBase
    {
        public CorValue(ICorDebugValue value)
            : base(value)
        {
            m_val = value;
        }

        [CLSCompliant(false)]
        public ICorDebugValue Raw
        {
            get
            {
                return m_val;
            }
        }

        /** The simple type of the value. */
        public CorElementType Type
        {
            get
            {
                CorElementType varType;
                m_val.GetType(out varType);
                return varType;
            }
        }

        /** Full runtime type of the object . */
        public CorType ExactType
        {
            get
            {
                ICorDebugValue2 v2 = (ICorDebugValue2)m_val;
                ICorDebugType dt;
                v2.GetExactType(out dt);
                return new CorType(dt);
            }
        }

        /** size of the value (in bytes). */
        public int Size
        {
            get
            {
                uint s = 0;
                m_val.GetSize(out s);
                return (int)s;
            }
        }

        /** Address of the value in the debuggee process. */
        public long Address
        {
            get
            {
                ulong addr = 0;
                m_val.GetAddress(out addr);
                return (long)addr;
            }
        }

        /** Breakpoint triggered when the value is modified. */
        public CorValueBreakpoint CreateBreakpoint()
        {
            ICorDebugValueBreakpoint bp = null;
            m_val.CreateBreakpoint(out bp);
            return new CorValueBreakpoint(bp);
        }

        // casting operations
        public CorReferenceValue CastToReferenceValue()
        {
            if (m_val is ICorDebugReferenceValue)
                return new CorReferenceValue((ICorDebugReferenceValue)m_val);
            else
                return null;
        }

        public CorHandleValue CastToHandleValue()
        {
            if (m_val is ICorDebugHandleValue)
                return new CorHandleValue((ICorDebugHandleValue)m_val);
            else
                return null;
        }

        public CorStringValue CastToStringValue()
        {
            return new CorStringValue((ICorDebugStringValue)m_val);
        }

        public CorObjectValue CastToObjectValue()
        {
            return new CorObjectValue((ICorDebugObjectValue)m_val);
        }

        public CorGenericValue CastToGenericValue()
        {
            if (m_val is ICorDebugGenericValue)
                return new CorGenericValue((ICorDebugGenericValue)m_val);
            else
                return null;
        }

        public CorBoxValue CastToBoxValue()
        {
            if (m_val is ICorDebugBoxValue)
                return new CorBoxValue((ICorDebugBoxValue)m_val);
            else
                return null;
        }

        public CorArrayValue CastToArrayValue()
        {
            if (m_val is ICorDebugArrayValue)
                return new CorArrayValue((ICorDebugArrayValue)m_val);
            else
                return null;
        }

        public CorHeapValue CastToHeapValue()
        {
            if (m_val is ICorDebugHeapValue)
                return new CorHeapValue((ICorDebugHeapValue)m_val);
            else
                return null;
        }

        internal ICorDebugValue m_val = null;

    } /* class Value */
}
