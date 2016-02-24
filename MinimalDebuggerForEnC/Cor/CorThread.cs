using MinimalDebuggerForEnC.NativeApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Cor
{
    public sealed class CorThread : WrapperBase
    {
        internal CorThread(ICorDebugThread thread)
            : base(thread)
        {
            m_th = thread;
        }

        internal ICorDebugThread GetInterface()
        {
            return m_th;
        }

        [CLSCompliant(false)]
        public ICorDebugThread Raw
        {
            get
            {
                return m_th;
            }
        }

        /** The process that this thread is in. */
        public CorProcess Process
        {
            get
            {
                ICorDebugProcess p = null;
                m_th.GetProcess(out p);
                return CorProcess.GetCorProcess(p);
            }
        }

        /** the OS id of the thread. */
        public int Id
        {
            get
            {
                uint id = 0;
                m_th.GetID(out id);
                return (int)id;
            }
        }

        /** The handle of the active part of the thread. */
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IntPtr Handle
        {
            get
            {
                IntPtr h = IntPtr.Zero;
                m_th.GetHandle(out h);
                return h;
            }
        }

        /** The AppDomain that owns the thread. */
        public CorAppDomain AppDomain
        {
            get
            {
                ICorDebugAppDomain ad = null;
                m_th.GetAppDomain(out ad);
                return new CorAppDomain(ad);
            }
        }

        /** Set the current debug state of the thread. */
        [CLSCompliant(false)]
        public CorDebugThreadState DebugState
        {
            get
            {
                CorDebugThreadState s = CorDebugThreadState.THREAD_RUN;
                m_th.GetDebugState(out s);
                return s;
            }
            set
            {
                m_th.SetDebugState(value);
            }
        }

        /** the user state. */
        [CLSCompliant(false)]
        public CorDebugUserState UserState
        {
            get
            {
                CorDebugUserState s = CorDebugUserState.USER_STOP_REQUESTED;
                m_th.GetUserState(out s);
                return s;
            }
        }

        /** the exception object which is currently being thrown by the thread. */
        public CorValue CurrentException
        {
            get
            {
                ICorDebugValue v = null;
                m_th.GetCurrentException(out v);
                return (v == null) ? null : new CorValue(v);
            }
        }

        /** gets the current custom notification object on the thread or null if
         * no such object exists.
         * */
        public CorValue CurrentNotification
        {
            get
            {
                ICorDebugThread4 th4 = (ICorDebugThread4)m_th;

                ICorDebugValue v = null;
                th4.GetCurrentCustomDebuggerNotification(out v);
                return (v == null) ? null : new CorValue(v);
            }
        }

        /// <summary>
        /// Returns true if this thread has an unhandled managed exception
        /// </summary>
        public bool HasUnhandledException
        {
            get
            {
                // This is only supported on ICorDebugThread4
                ICorDebugThread4 th4 = m_th as ICorDebugThread4;
                if (th4 == null)
                    throw new NotSupportedException();
                else
                {
                    int ret = th4.HasUnhandledException();
                    if (ret == (int)HResult.S_OK)
                        return true;
                    else if (ret == (int)HResult.S_FALSE)
                        return false;
                    else
                        Marshal.ThrowExceptionForHR(ret);
                    // unreachable
                    throw new Exception();
                }
            }
        }

        /** 
         * Clear the current exception object, preventing it from being thrown.
         */
        public void ClearCurrentException()
        {
            m_th.ClearCurrentException();
        }

        /** 
         * Intercept the current exception.
         */
        public void InterceptCurrentException(CorFrame frame)
        {
            ICorDebugThread2 m_th2 = (ICorDebugThread2)m_th;
            m_th2.InterceptCurrentException(frame.m_frame);
        }

        /** 
         * create a stepper object relative to the active frame in this thread.
         */
        public CorStepper CreateStepper()
        {
            ICorDebugStepper s = null;
            m_th.CreateStepper(out s);
            return new CorStepper(s);
        }

        /** All stack chains in the thread. */
        public IEnumerable Chains
        {
            get
            {
                ICorDebugChainEnum ec = null;
                m_th.EnumerateChains(out ec);
                return (ec == null) ? null : new CorChainEnumerator(ec);
            }
        }

        /** The most recent chain in the thread, if any. */
        public CorChain ActiveChain
        {
            get
            {
                ICorDebugChain ch = null;
                m_th.GetActiveChain(out ch);
                return ch == null ? null : new CorChain(ch);
            }
        }

        /** Get the active frame. */
        public CorFrame ActiveFrame
        {
            get
            {
                ICorDebugFrame f = null;
                m_th.GetActiveFrame(out f);
                return f == null ? null : new CorFrame(f);
            }
        }

        /** Get the register set for the active part of the thread. */
        public CorRegisterSet RegisterSet
        {
            get
            {
                ICorDebugRegisterSet r = null;
                m_th.GetRegisterSet(out r);
                return r == null ? null : new CorRegisterSet(r);
            }
        }

        /** Creates an evaluation object. */
        public CorEval CreateEval()
        {
            ICorDebugEval e = null;
            m_th.CreateEval(out e);
            return e == null ? null : new CorEval(e);
        }

        /** Get the runtime thread object. */
        public CorValue ThreadVariable
        {
            get
            {
                ICorDebugValue v = null;
                m_th.GetObject(out v);
                return new CorValue(v);
            }
        }

        public CorActiveFunction[] GetActiveFunctions()
        {
            ICorDebugThread2 m_th2 = (ICorDebugThread2)m_th;
            UInt32 pcFunctions;
            m_th2.GetActiveFunctions(0, out pcFunctions, null);
            COR_ACTIVE_FUNCTION[] afunctions = new COR_ACTIVE_FUNCTION[pcFunctions];
            m_th2.GetActiveFunctions(pcFunctions, out pcFunctions, afunctions);
            CorActiveFunction[] caf = new CorActiveFunction[pcFunctions];
            for (int i = 0; i < pcFunctions; ++i)
            {
                caf[i] = new CorActiveFunction((int)afunctions[i].ilOffset,
                                               new CorFunction((ICorDebugFunction)afunctions[i].pFunction),
                                               afunctions[i].pModule == null ? null : new CorModule(afunctions[i].pModule)
                                               );
            }
            return caf;
        }

        public bool IsV3
        {
            get
            {
                ICorDebugThread3 th3 = m_th as ICorDebugThread3;
                if (th3 == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /** 
         * If PureV3StackWalk is specified, then this method returns a CorStackWalk, which does not expose
         * ICorDebugInternalFrames.  If ExtendedV3StackWalk is specified, then this method returns a 
         * CorStackWalkEx, which derives from CorStackWalk and interleaves ICorDebugInternalFrames.
         */
        public CorStackWalk CreateStackWalk(CorStackWalkType type)
        {
            ICorDebugThread3 th3 = m_th as ICorDebugThread3;
            if (th3 == null)
            {
                return null;
            }

            ICorDebugStackWalk s = null;
            th3.CreateStackWalk(out s);
            if (type == CorStackWalkType.PureV3StackWalk)
            {
                return new CorStackWalk(s, this);
            }
            else
            {
                return new CorStackWalkEx(s, this);
            }
        }
        public CorStackWalk CreateStackWalk()
        {
            return CreateStackWalk(CorStackWalkType.PureV3StackWalk);
        }

        public CorFrame[] GetActiveInternalFrames()
        {
            ICorDebugThread3 th3 = (ICorDebugThread3)m_th;

            UInt32 cInternalFrames = 0;
            th3.GetActiveInternalFrames(0, out cInternalFrames, null);

            ICorDebugInternalFrame2[] ppInternalFrames = new ICorDebugInternalFrame2[cInternalFrames];
            th3.GetActiveInternalFrames(cInternalFrames, out cInternalFrames, ppInternalFrames);

            CorFrame[] corFrames = new CorFrame[cInternalFrames];
            for (int i = 0; i < cInternalFrames; i++)
            {
                corFrames[i] = new CorFrame(ppInternalFrames[i] as ICorDebugFrame);
            }
            return corFrames;
        }

        ///<summary>
        ///Returns an array of objects which this thread is blocked on
        ///</summary>
        public CorBlockingObject[] GetBlockingObjects()
        {
            ICorDebugThread4 th4 = m_th as ICorDebugThread4;
            if (th4 == null)
                throw new NotSupportedException();
            ICorDebugEnumBlockingObject blockingObjectEnumerator;
            th4.GetBlockingObjects(out blockingObjectEnumerator);
            uint countBlockingObjects;
            blockingObjectEnumerator.GetCount(out countBlockingObjects);
            CorDebugBlockingObject[] rawBlockingObjects = new CorDebugBlockingObject[countBlockingObjects];
            uint countFetched;
            blockingObjectEnumerator.Next(countBlockingObjects, rawBlockingObjects, out countFetched);
            Debug.Assert(countFetched == countBlockingObjects);
            CorBlockingObject[] blockingObjects = new CorBlockingObject[countBlockingObjects];
            for (int i = 0; i < countBlockingObjects; i++)
            {
                blockingObjects[i].BlockingObject = new CorValue(rawBlockingObjects[i].BlockingObject);
                if (rawBlockingObjects[i].Timeout == uint.MaxValue)
                {
                    blockingObjects[i].Timeout = TimeSpan.MaxValue;
                }
                else
                {
                    blockingObjects[i].Timeout = TimeSpan.FromMilliseconds(rawBlockingObjects[i].Timeout);
                }
                blockingObjects[i].BlockingReason = rawBlockingObjects[i].BlockingReason;
            }
            return blockingObjects;
        }

        private ICorDebugThread m_th;

    }
}
