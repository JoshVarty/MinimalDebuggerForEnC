using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnC.Cor
{
    public enum CorCorDebugRegister
    {
        Eip = 0,                           // REGISTER_X86_EIP = 0,
        Esp,                               // REGISTER_X86_ESP,
        Ebp,                               // REGISTER_X86_EBP,

        Eax,                               // REGISTER_X86_EAX,
        Ecx,                               // REGISTER_X86_ECX,
        Edx,                               // REGISTER_X86_EDX,
        Ebx,                               // REGISTER_X86_EBX,

        Esi,                               // REGISTER_X86_ESI,
        Edi,                               // REGISTER_X86_EDI,

        FPstack0,                          // REGISTER_X86_FPSTACK_0,
        FPstack1,                          // REGISTER_X86_FPSTACK_1,
        FPstack2,                          // REGISTER_X86_FPSTACK_2,
        FPstack3,                          // REGISTER_X86_FPSTACK_3,
        FPstack4,                          // REGISTER_X86_FPSTACK_4,
        FPstack5,                          // REGISTER_X86_FPSTACK_5,
        FPstack6,                          // REGISTER_X86_FPSTACK_6,
        FPstack7,                          // REGISTER_X86_FPSTACK_7,

        RegisterMax // this needs to be last enum!
    };
}
