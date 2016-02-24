using MinimalDebuggerForEnC.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Cor
{
    public sealed class CorChain : WrapperBase
    {
        internal CorChain(ICorDebugChain chain)
            : base(chain)
        {
            m_chain = chain;
        }

        [CLSCompliant(false)]
        public ICorDebugChain Raw
        {
            get
            {
                return m_chain;
            }
        }

        public CorFrame ActiveFrame
        {
            get
            {
                ICorDebugFrame iframe;
                m_chain.GetActiveFrame(out iframe);
                return (iframe == null ? null : new CorFrame(iframe));
            }
        }

        public CorChain Callee
        {
            get
            {
                ICorDebugChain ichain;
                m_chain.GetCallee(out ichain);
                return (ichain == null ? null : new CorChain(ichain));
            }
        }

        public CorChain Caller
        {
            get
            {
                ICorDebugChain ichain;
                m_chain.GetCaller(out ichain);
                return (ichain == null ? null : new CorChain(ichain));
            }
        }

        public CorContext Context
        {
            get
            {
                ICorDebugContext icontext;
                m_chain.GetContext(out icontext);
                return (icontext == null ? null : new CorContext(icontext));
            }
        }

        public CorChain Next
        {
            get
            {
                ICorDebugChain ichain;
                m_chain.GetNext(out ichain);
                return (ichain == null ? null : new CorChain(ichain));
            }
        }

        public CorChain Previous
        {
            get
            {
                ICorDebugChain ichain;
                m_chain.GetPrevious(out ichain);
                return (ichain == null ? null : new CorChain(ichain));
            }
        }

        [CLSCompliant(false)]
        public CorDebugChainReason Reason
        {
            get
            {
                CorDebugChainReason reason;
                m_chain.GetReason(out reason);
                return reason;
            }
        }

        public CorRegisterSet RegisterSet
        {
            get
            {
                ICorDebugRegisterSet r = null;
                m_chain.GetRegisterSet(out r);
                return r == null ? null : new CorRegisterSet(r);
            }
        }

        public void GetStackRange(out Int64 pStart, out Int64 pEnd)
        {
            UInt64 start = 0;
            UInt64 end = 0;
            m_chain.GetStackRange(out start, out end);
            pStart = (Int64)start;
            pEnd = (Int64)end;
        }

        public CorThread Thread
        {
            get
            {
                ICorDebugThread ithread;
                m_chain.GetThread(out ithread);
                return (ithread == null ? null : new CorThread(ithread));
            }
        }

        public bool IsManaged
        {
            get
            {
                int managed;
                m_chain.IsManaged(out managed);
                return (managed != 0 ? true : false);
            }
        }

        public IEnumerable Frames
        {
            get
            {
                ICorDebugFrameEnum ef = null;
                m_chain.EnumerateFrames(out ef);
                return (ef == null) ? null : new CorFrameEnumerator(ef);
            }
        }

        private ICorDebugChain m_chain;
    }
}
