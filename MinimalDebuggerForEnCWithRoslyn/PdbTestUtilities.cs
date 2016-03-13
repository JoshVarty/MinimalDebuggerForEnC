using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.DiaSymReader;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnCWithRoslyn
{
    internal static class PdbTestUtilities
    {
        public static EditAndContinueMethodDebugInformation GetEncMethodDebugInfo(this ISymUnmanagedReader symReader, MethodDefinitionHandle handle)
        {
            var cdi = CustomDebugInfoUtilities.GetCustomDebugInfoBytes(symReader, handle, methodVersion: 1);
            if (cdi == null)
            {
                return default(EditAndContinueMethodDebugInformation);
            }

            return GetEncMethodDebugInfo(cdi);
        }

        public static EditAndContinueMethodDebugInformation GetEncMethodDebugInfo(byte[] customDebugInfoBlob)
        {
            return EditAndContinueMethodDebugInformation.Create(
                CustomDebugInfoUtilities.GetEditAndContinueLocalSlotMapRecord(customDebugInfoBlob),
                CustomDebugInfoUtilities.GetEditAndContinueLambdaMapRecord(customDebugInfoBlob));
        }

        public static string GetTokenToLocationMap(Compilation compilation, bool maskToken = false)
        {
            using (var exebits = new MemoryStream())
            {
                using (var pdbbits = new MemoryStream())
                {
                    compilation.Emit(exebits, pdbbits);
                    return Token2SourceLineExporter.TokenToSourceMap2Xml(pdbbits, maskToken);
                }
            }
        }
    }

    public static class CustomDebugInfoUtilities
    {
        public static byte[] GetCustomDebugInfoBytes(ISymUnmanagedReader reader, MethodDefinitionHandle handle, int methodVersion)
        {
            return reader.GetCustomDebugInfoBytes(MetadataTokens.GetToken(handle), methodVersion);
        }

        public static ImmutableArray<byte> GetEditAndContinueLocalSlotMapRecord(byte[] customDebugInfoBlob)
        {
            return CustomDebugInfoReader.TryGetCustomDebugInfoRecord(customDebugInfoBlob, CustomDebugInfoKind.EditAndContinueLocalSlotMap);
        }

        public static ImmutableArray<byte> GetEditAndContinueLambdaMapRecord(byte[] customDebugInfoBlob)
        {
            return CustomDebugInfoReader.TryGetCustomDebugInfoRecord(customDebugInfoBlob, CustomDebugInfoKind.EditAndContinueLambdaMap);
        }
    }
}
