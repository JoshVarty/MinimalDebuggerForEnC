using MinimalDebuggerForEnC.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Cor
{
    public sealed class CorContext : WrapperBase
    {
        internal CorContext(ICorDebugContext context) : base(context)
        {
            m_context = context;
        }

        [CLSCompliant(false)]
        public ICorDebugContext Raw
        {
            get
            {
                return m_context;
            }
        }

        private ICorDebugContext m_context;
    }
}
