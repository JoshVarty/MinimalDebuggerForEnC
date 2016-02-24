using MinimalDebuggerForEnC.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Cor
{
    public sealed class CorFrame : WrapperBase
    {
        internal CorFrame(ICorDebugFrame frame)
            : base(frame)
        {
            m_frame = frame;
        }

        [CLSCompliant(false)]
        public ICorDebugFrame Raw
        {
            get
            {
                return m_frame;
            }
        }

        public CorStepper CreateStepper()
        {
            ICorDebugStepper istepper;
            m_frame.CreateStepper(out istepper);
            return (istepper == null ? null : new CorStepper(istepper));
        }

        public CorFrame Callee
        {
            get
            {
                ICorDebugFrame iframe;
                m_frame.GetCallee(out iframe);
                return (iframe == null ? null : new CorFrame(iframe));
            }
        }

        public CorFrame Caller
        {
            get
            {
                ICorDebugFrame iframe;
                m_frame.GetCaller(out iframe);
                return (iframe == null ? null : new CorFrame(iframe));
            }
        }

        public CorChain Chain
        {
            get
            {
                ICorDebugChain ichain;
                m_frame.GetChain(out ichain);
                return (ichain == null ? null : new CorChain(ichain));
            }
        }

        public CorCode Code
        {
            get
            {
                ICorDebugCode icode;
                m_frame.GetCode(out icode);
                return (icode == null ? null : new CorCode(icode));
            }
        }

        public CorFunction Function
        {
            get
            {
                ICorDebugFunction ifunction;
                try
                {
                    m_frame.GetFunction(out ifunction);
                }
                catch (System.Runtime.InteropServices.COMException e)
                {
                    if (e.ErrorCode == (int)HResult.CORDBG_E_CODE_NOT_AVAILABLE)
                    {
                        return null;
                    }
                    else
                    {
                        throw;
                    }
                }

                return (ifunction == null ? null : new CorFunction(ifunction));
            }
        }

        public int FunctionToken
        {
            get
            {
                uint token;
                m_frame.GetFunctionToken(out token);
                return (int)token;
            }
        }

        public CorFrameType FrameType
        {
            get
            {
                ICorDebugILFrame ilframe = GetILFrame();
                if (ilframe != null)
                    return CorFrameType.ILFrame;

                ICorDebugInternalFrame iframe = GetInternalFrame();
                if (iframe != null)
                    return CorFrameType.InternalFrame;

                ICorDebugRuntimeUnwindableFrame ruf = GetRuntimeUnwindableFrame();
                if (ruf != null)
                    return CorFrameType.RuntimeUnwindableFrame;
                return CorFrameType.NativeFrame;
            }
        }

        [CLSCompliant(false)]
        public CorDebugInternalFrameType InternalFrameType
        {
            get
            {
                ICorDebugInternalFrame iframe = GetInternalFrame();
                CorDebugInternalFrameType ft;

                if (iframe == null)
                    throw new CorException("Cannot get frame type on non-internal frame");

                iframe.GetFrameType(out ft);
                return ft;
            }
        }

        [CLSCompliant(false)]
        public ulong Address
        {
            get
            {
                ICorDebugInternalFrame iframe = GetInternalFrame();
                if (iframe == null)
                {
                    throw new CorException("Cannot get the frame address on non-internal frame");
                }

                ulong address = 0;
                ICorDebugInternalFrame2 iframe2 = (ICorDebugInternalFrame2)iframe;
                iframe2.GetAddress(out address);
                return address;
            }
        }

        public bool IsCloserToLeaf(CorFrame frameToCompare)
        {
            ICorDebugInternalFrame2 iFrame2 = m_frame as ICorDebugInternalFrame2;
            if (iFrame2 == null)
            {
                throw new ArgumentException("The this object is not an ICorDebugInternalFrame");
            }

            int isCloser = 0;
            iFrame2.IsCloserToLeaf(frameToCompare.m_frame, out isCloser);
            return (isCloser == 0 ? false : true);
        }

        [CLSCompliant(false)]
        public void GetStackRange(out UInt64 startOffset, out UInt64 endOffset)
        {
            m_frame.GetStackRange(out startOffset, out endOffset);
        }

        [CLSCompliant(false)]
        public void GetIP(out uint offset, out CorDebugMappingResult mappingResult)
        {
            ICorDebugILFrame ilframe = GetILFrame();
            if (ilframe == null)
            {
                offset = 0;
                mappingResult = CorDebugMappingResult.MAPPING_NO_INFO;
            }
            else
                ilframe.GetIP(out offset, out mappingResult);
        }

        public void SetIP(int offset)
        {
            ICorDebugILFrame ilframe = GetILFrame();
            if (ilframe == null)
                throw new CorException("Cannot set an IP on non-il frame");
            ilframe.SetIP((uint)offset);
        }

        public bool CanSetIP(int offset)
        {
            ICorDebugILFrame ilframe = GetILFrame();
            if (ilframe == null)
                return false;
            return (ilframe.CanSetIP((uint)offset) == (int)HResult.S_OK);
        }

        public bool CanSetIP(int offset, out int hresult)
        {
            ICorDebugILFrame ilframe = GetILFrame();
            if (ilframe == null)
            {
                hresult = (int)HResult.E_FAIL;
                return false;
            }
            hresult = ilframe.CanSetIP((uint)offset);
            return (hresult == (int)HResult.S_OK);
        }

        [CLSCompliant(false)]
        public void GetNativeIP(out uint offset)
        {
            ICorDebugNativeFrame nativeFrame = m_frame as ICorDebugNativeFrame;
            Debug.Assert(nativeFrame != null);
            nativeFrame.GetIP(out offset);
        }
        public bool IsChild
        {
            get
            {
                ICorDebugNativeFrame2 nativeFrame2 = m_frame as ICorDebugNativeFrame2;
                if (nativeFrame2 == null)
                {
                    return false;
                }

                int isChild = 0;
                nativeFrame2.IsChild(out isChild);
                return (isChild == 0 ? false : true);
            }
        }

        [CLSCompliant(false)]
        public bool IsMatchingParentFrame(CorFrame parentFrame)
        {
            if (!this.IsChild)
            {
                return false;
            }
            ICorDebugNativeFrame2 nativeFrame2 = m_frame as ICorDebugNativeFrame2;
            Debug.Assert(nativeFrame2 != null);

            ICorDebugNativeFrame2 nativeParentFrame2 = parentFrame.m_frame as ICorDebugNativeFrame2;
            if (nativeParentFrame2 == null)
            {
                return false;
            }

            int isParent = 0;
            nativeFrame2.IsMatchingParentFrame(nativeParentFrame2, out isParent);
            return (isParent == 0 ? false : true);
        }

        [CLSCompliant(false)]
        public uint CalleeStackParameterSize
        {
            get
            {
                ICorDebugNativeFrame2 nativeFrame2 = m_frame as ICorDebugNativeFrame2;
                Debug.Assert(nativeFrame2 != null);

                uint paramSize = 0;
                nativeFrame2.GetCalleeStackParameterSize(out paramSize);
                return paramSize;
            }
        }

        public CorValue GetLocalVariable(int index)
        {
            ICorDebugILFrame ilframe = GetILFrame();
            if (ilframe == null)
                return null;

            ICorDebugValue value;
            try
            {
                ilframe.GetLocalVariable((uint)index, out value);
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                // If you are stopped in the Prolog, the variable may not be available.
                // CORDBG_E_IL_VAR_NOT_AVAILABLE is returned after dubugee triggers StackOverflowException
                if (e.ErrorCode == (int)HResult.CORDBG_E_IL_VAR_NOT_AVAILABLE)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
            return (value == null) ? null : new CorValue(value);
        }

        public int GetLocalVariablesCount()
        {
            ICorDebugILFrame ilframe = GetILFrame();
            if (ilframe == null)
                return -1;

            ICorDebugValueEnum ve;
            ilframe.EnumerateLocalVariables(out ve);
            uint count;
            ve.GetCount(out count);
            return (int)count;
        }

        public CorValue GetArgument(int index)
        {
            ICorDebugILFrame ilframe = GetILFrame();
            if (ilframe == null)
                return null;


            ICorDebugValue value;
            ilframe.GetArgument((uint)index, out value);
            return (value == null) ? null : new CorValue(value);
        }

        public int GetArgumentCount()
        {
            ICorDebugILFrame ilframe = GetILFrame();
            if (ilframe == null)
                return -1;

            ICorDebugValueEnum ve;
            ilframe.EnumerateArguments(out ve);
            uint count;
            ve.GetCount(out count);
            return (int)count;
        }

        public void RemapFunction(int newILOffset)
        {
            ICorDebugILFrame ilframe = GetILFrame();
            if (ilframe == null)
                throw new CorException("Cannot remap on non-il frame.");
            ICorDebugILFrame2 ilframe2 = (ICorDebugILFrame2)ilframe;
            ilframe2.RemapFunction((uint)newILOffset);
        }

        private ICorDebugILFrame GetILFrame()
        {
            if (!m_ilFrameCached)
            {
                m_ilFrameCached = true;
                m_ilFrame = m_frame as ICorDebugILFrame;

            }
            return m_ilFrame;
        }

        private ICorDebugInternalFrame GetInternalFrame()
        {
            if (!m_iFrameCached)
            {
                m_iFrameCached = true;

                m_iFrame = m_frame as ICorDebugInternalFrame;
            }
            return m_iFrame;
        }

        private ICorDebugRuntimeUnwindableFrame GetRuntimeUnwindableFrame()
        {
            if (!m_ruFrameCached)
            {
                m_ruFrameCached = true;

                m_ruFrame = m_frame as ICorDebugRuntimeUnwindableFrame;
            }
            return m_ruFrame;
        }
        // 'TypeParameters' returns an enumerator that goes yields generic args from
        // both the class and the method. To enumerate just the generic args on the 
        // method, we need to skip past the class args. We have to get that skip value
        // from the metadata. This is a helper function to efficiently get an enumerator that skips
        // to a given spot (likely past the class generic args). 
        public IEnumerable GetTypeParamEnumWithSkip(int skip)
        {
            if (skip < 0)
            {
                throw new ArgumentException("Skip parameter must be positive");
            }
            IEnumerable e = this.TypeParameters;
            Debug.Assert(e is CorTypeEnumerator);

            // Skip will throw if we try to skip the whole collection
            int total = (e as CorTypeEnumerator).Count;
            if (skip >= total)
            {
                return new CorTypeEnumerator(null); // empty.
            }

            (e as CorTypeEnumerator).Skip(skip);
            return e;
        }

        public IEnumerable TypeParameters
        {
            get
            {
                ICorDebugTypeEnum icdte = null;
                ICorDebugILFrame ilf = GetILFrame();

                (ilf as ICorDebugILFrame2).EnumerateTypeParameters(out icdte);
                return new CorTypeEnumerator(icdte);        // icdte can be null, is handled by enumerator
            }
        }



        private ICorDebugILFrame m_ilFrame = null;
        private bool m_ilFrameCached = false;

        private ICorDebugInternalFrame m_iFrame = null;
        private bool m_iFrameCached = false;
        private ICorDebugRuntimeUnwindableFrame m_ruFrame = null;
        private bool m_ruFrameCached = false;

        internal ICorDebugFrame m_frame;
    }
}
