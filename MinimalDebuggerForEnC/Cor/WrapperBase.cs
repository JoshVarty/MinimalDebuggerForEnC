using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Cor
{
    /// <summary>
    /// A base class for all COM wrappers. 
    /// </summary>
    public abstract class WrapperBase : MarshalByRefObject
    {
        /// <summary>
        /// Initializes an instance of the Wrapper class.
        /// </summary>
        /// <param name="value">COM object to wrap</param>
        protected WrapperBase(Object value)
        {
            Debug.Assert(value != null);
            coobject = value;
        }

        /// <summary cref="System.Object.Equals(Object)">
        /// </summary>
        public override bool Equals(Object value)
        {
            if (!(value is WrapperBase))
                return false;
            return ((value as WrapperBase).coobject == this.coobject);
        }

        /// <summary cref="System.Object.GetHashCode">
        /// </summary>
        public override int GetHashCode()
        {
            return coobject.GetHashCode();
        }

        /// <summary>
        /// Override also equality operator so we compare 
        /// COM objects inside instead of wrapper references.
        /// </summary>
        /// <param name="operand">first operand</param>
        /// <param name="operand2">second operand</param>
        /// <returns>true if inner COM objects are the same, false otherwise</returns>
        public static bool operator ==(WrapperBase operand, WrapperBase operand2)
        {
            if (Object.ReferenceEquals(operand, operand2))
                return true;

            if (Object.ReferenceEquals(operand, null))               // this means that operand==null && operand2 is not null 
                return false;

            return operand.Equals(operand2);
        }

        /// <summary>
        /// Override also inequality operator so we compare 
        /// COM objects inside instead of wrapper references.
        /// </summary>
        /// <param name="operand">first operand</param>
        /// <param name="operand2">second operand</param>
        /// <returns>true if inner COM objects are different, true otherwise</returns>
        public static bool operator !=(WrapperBase operand, WrapperBase operand2)
        {
            return !(operand == operand2);
        }

        private Object coobject;
    }
}
