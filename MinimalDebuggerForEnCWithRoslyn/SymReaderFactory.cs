using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using Microsoft.DiaSymReader;
using System.IO;
using System.Collections.Immutable;

namespace MinimalDebuggerForEnCWithRoslyn
{
    public static class SymReaderFactory
    {
        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        [DllImport("Microsoft.DiaSymReader.Native.x86.dll", EntryPoint = "CreateSymReader")]
        private extern static void CreateSymReader32(ref Guid id, [MarshalAs(UnmanagedType.IUnknown)]out object symReader);

        [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
        [DllImport("Microsoft.DiaSymReader.Native.amd64.dll", EntryPoint = "CreateSymReader")]
        private extern static void CreateSymReader64(ref Guid id, [MarshalAs(UnmanagedType.IUnknown)]out object symReader);

        private static ISymUnmanagedReader3 CreateNativeSymReader(Stream pdbStream, object metadataImporter)
        {
            object symReader = null;

            var guid = default(Guid);
            if (IntPtr.Size == 4)
            {
                CreateSymReader32(ref guid, out symReader);
            }
            else
            {
                CreateSymReader64(ref guid, out symReader);
            }

            var reader = (ISymUnmanagedReader3)symReader;
            int hr = reader.Initialize(metadataImporter, null, null, new ComStreamWrapper(pdbStream));
            SymUnmanagedReaderExtensions.ThrowExceptionForHR(hr);
            return reader;
        }

        private static ISymUnmanagedReader CreatePortableSymReader(Stream pdbStream, object metadataImporter)
        {
            throw new NotImplementedException("I can't implement SymBinder yet");

            //TODO: Implement SymbBinder
            //var binder = new PortablePdb.SymBinder();

            //ISymUnmanagedReader reader;
            //int hr = binder.GetReaderFromStream(metadataImporter, new ComStreamWrapper(pdbStream), out reader);
            //SymUnmanagedReaderExtensions.ThrowExceptionForHR(hr);

            //return reader;
        }

        public static ISymUnmanagedReader CreateReader(byte[] pdbImage, byte[] peImageOpt = null)
        {
            return CreateReader(new MemoryStream(pdbImage), (peImageOpt != null) ? new PEReader(new MemoryStream(peImageOpt)) : null);
        }

        public static ISymUnmanagedReader CreateReader(ImmutableArray<byte> pdbImage, ImmutableArray<byte> peImageOpt = default(ImmutableArray<byte>))
        {
            return CreateReader(new MemoryStream(pdbImage.ToArray()), (peImageOpt.IsDefault) ? null : new PEReader(peImageOpt));
        }

        public static ISymUnmanagedReader CreateReader(Stream pdbStream, Stream peStreamOpt = null)
        {
            return CreateReader(pdbStream, (peStreamOpt != null) ? new PEReader(peStreamOpt) : null);
        }

        public static ISymUnmanagedReader CreateReader(Stream pdbStream, PEReader peReaderOpt)
        {
            return CreateReader(pdbStream, peReaderOpt?.GetMetadataReader(), peReaderOpt);
        }

        public static ISymUnmanagedReader CreateReader(Stream pdbStream, MetadataReader metadataReaderOpt, IDisposable metadataMemoryOwnerOpt)
        {
            return CreateReaderImpl(pdbStream, metadataImporter: new DummyMetadataImport(metadataReaderOpt, metadataMemoryOwnerOpt));
        }

        public static ISymUnmanagedReader CreateReaderImpl(Stream pdbStream, object metadataImporter)
        {
            pdbStream.Position = 0;
            bool isPortable = pdbStream.ReadByte() == 'B' && pdbStream.ReadByte() == 'S' && pdbStream.ReadByte() == 'J' && pdbStream.ReadByte() == 'B';
            pdbStream.Position = 0;

            if (isPortable)
            {
                return CreatePortableSymReader(pdbStream, metadataImporter);
            }
            else
            {
                return CreateNativeSymReader(pdbStream, metadataImporter);
            }
        }
    }
}
