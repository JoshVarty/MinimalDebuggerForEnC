using MinimalDebuggerForEnC.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Cor
{
    /// <summary>
    /// ICorDebugProcess wrapper.
    /// </summary>
    public sealed class CorProcess : WrapperBase
    {
        private readonly ICorDebugProcess coprocess;

        /// <summary>
        /// Creates a new ICorDebugProcess wrapper.
        /// </summary>
        /// <param name="coprocess">COM process object</param>
        internal CorProcess(ICorDebugProcess coprocess) : base(coprocess)
        {
            this.coprocess = coprocess;
        }
    }
}
