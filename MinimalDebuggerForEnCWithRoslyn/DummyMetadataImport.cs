using Microsoft.Samples.Debugging.CorMetadata.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinimalDebuggerForEnCWithRoslyn
{
    internal sealed class DummyMetadataImport : IMetadataImport, IDisposable
    {
        private readonly MetadataReader _metadataReaderOpt;
        private readonly IDisposable _metadataOwnerOpt;
        private readonly List<GCHandle> _pinnedBuffers;

        public DummyMetadataImport(MetadataReader metadataReaderOpt, IDisposable metadataOwnerOpt)
        {
            _metadataReaderOpt = metadataReaderOpt;
            _pinnedBuffers = new List<GCHandle>();
            _metadataOwnerOpt = metadataOwnerOpt;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            _metadataOwnerOpt?.Dispose();

            foreach (var pinnedBuffer in _pinnedBuffers)
            {
                pinnedBuffer.Free();
            }
        }

        ~DummyMetadataImport()
        {
            Dispose(false);
        }

        [PreserveSig]
        public unsafe int GetSigFromToken(
            int tkSignature,    // Signature token.
            out byte* ppvSig,   // return pointer to signature blob
            out int pcbSig)     // return size of signature
        {
            if (_metadataReaderOpt == null)
            {
                throw new NotSupportedException("Metadata not available");
            }

            var sig = _metadataReaderOpt.GetStandaloneSignature((StandaloneSignatureHandle)MetadataTokens.Handle(tkSignature));
            var signature = _metadataReaderOpt.GetBlobBytes(sig.Signature);

            GCHandle pinnedBuffer = GCHandle.Alloc(signature, GCHandleType.Pinned);
            ppvSig = (byte*)pinnedBuffer.AddrOfPinnedObject();
            pcbSig = signature.Length;

            _pinnedBuffers.Add(pinnedBuffer);
            return 0;
        }

        public void GetTypeDefProps(
            int typeDefinition,
            [MarshalAs(UnmanagedType.LPWStr), Out]StringBuilder qualifiedName,
            int qualifiedNameBufferLength,
            out int qualifiedNameLength,
            [MarshalAs(UnmanagedType.U4)]out TypeAttributes attributes,
            out int baseType)
        {
            if (_metadataReaderOpt == null)
            {
                throw new NotSupportedException("Metadata not available");
            }

            var handle = (TypeDefinitionHandle)MetadataTokens.Handle(typeDefinition);
            var typeDef = _metadataReaderOpt.GetTypeDefinition(handle);

            if (qualifiedName != null)
            {
                qualifiedName.Clear();

                if (!typeDef.Namespace.IsNil)
                {
                    qualifiedName.Append(_metadataReaderOpt.GetString(typeDef.Namespace));
                    qualifiedName.Append('.');
                }

                qualifiedName.Append(_metadataReaderOpt.GetString(typeDef.Name));
                qualifiedNameLength = qualifiedName.Length;
            }
            else
            {
                qualifiedNameLength =
                    (typeDef.Namespace.IsNil ? 0 : _metadataReaderOpt.GetString(typeDef.Namespace).Length + 1) +
                    _metadataReaderOpt.GetString(typeDef.Name).Length;
            }

            baseType = MetadataTokens.GetToken(typeDef.BaseType);
            attributes = typeDef.Attributes;
        }

        public void GetTypeRefProps(
            int typeReference,
            out int resolutionScope,
            [MarshalAs(UnmanagedType.LPWStr), Out]StringBuilder qualifiedName,
            int qualifiedNameBufferLength,
            out int qualifiedNameLength)
        {
            if (_metadataReaderOpt == null)
            {
                throw new NotSupportedException("Metadata not available");
            }

            var handle = (TypeReferenceHandle)MetadataTokens.Handle(typeReference);
            var typeRef = _metadataReaderOpt.GetTypeReference(handle);

            if (qualifiedName != null)
            {
                qualifiedName.Clear();

                if (!typeRef.Namespace.IsNil)
                {
                    qualifiedName.Append(_metadataReaderOpt.GetString(typeRef.Namespace));
                    qualifiedName.Append('.');
                }

                qualifiedName.Append(_metadataReaderOpt.GetString(typeRef.Name));
                qualifiedNameLength = qualifiedName.Length;
            }
            else
            {
                qualifiedNameLength =
                    (typeRef.Namespace.IsNil ? 0 : _metadataReaderOpt.GetString(typeRef.Namespace).Length + 1) +
                    _metadataReaderOpt.GetString(typeRef.Name).Length;
            }

            resolutionScope = MetadataTokens.GetToken(typeRef.ResolutionScope);
        }

        #region Not Implemented

        public void ResetEnum(uint handleEnum, uint ulongPos)
        {
            throw new NotImplementedException();
        }

        public uint ResolveTypeRef(uint tr, [In]ref Guid riid, [MarshalAs(UnmanagedType.Interface)]out object ppIScope)
        {
            throw new NotImplementedException();
        }

        public void CloseEnum(IntPtr hEnum)
        {
            throw new NotImplementedException();
        }

        public void CountEnum(IntPtr hEnum, out int pulCount)
        {
            throw new NotImplementedException();
        }

        public void ResetEnum(IntPtr hEnum, int ulPos)
        {
            throw new NotImplementedException();
        }

        public void EnumTypeDefs(ref IntPtr phEnum, out int rTypeDefs, uint cMax, out uint pcTypeDefs)
        {
            throw new NotImplementedException();
        }

        public void EnumInterfaceImpls_(IntPtr phEnum, int td)
        {
            throw new NotImplementedException();
        }

        public void EnumTypeRefs_()
        {
            throw new NotImplementedException();
        }

        public void FindTypeDefByName([In, MarshalAs(UnmanagedType.LPWStr)] string szTypeDef, [In] int tkEnclosingClass, [Out] out int token)
        {
            throw new NotImplementedException();
        }

        public void GetScopeProps([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder szName, [In] int cchName, out int pchName, out Guid mvid)
        {
            throw new NotImplementedException();
        }

        public void GetModuleFromScope_()
        {
            throw new NotImplementedException();
        }

        public void GetInterfaceImplProps_()
        {
            throw new NotImplementedException();
        }

        public void ResolveTypeRef(int tr, ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object scope, out int typeDef)
        {
            throw new NotImplementedException();
        }

        public void EnumMembers_()
        {
            throw new NotImplementedException();
        }

        public void EnumMembersWithName_()
        {
            throw new NotImplementedException();
        }

        public void EnumMethods(ref IntPtr phEnum, int cl, out int mdMethodDef, int cMax, out int pcTokens)
        {
            throw new NotImplementedException();
        }

        public void EnumMethodsWithName_()
        {
            throw new NotImplementedException();
        }

        public void EnumFields(ref IntPtr phEnum, int cl, out int mdFieldDef, int cMax, out uint pcTokens)
        {
            throw new NotImplementedException();
        }

        public void EnumFieldsWithName_()
        {
            throw new NotImplementedException();
        }

        public void EnumParams(ref IntPtr phEnum, int mdMethodDef, out int mdParamDef, int cMax, out uint pcTokens)
        {
            throw new NotImplementedException();
        }

        public void EnumMemberRefs_()
        {
            throw new NotImplementedException();
        }

        public void EnumMethodImpls_()
        {
            throw new NotImplementedException();
        }

        public void EnumPermissionSets_()
        {
            throw new NotImplementedException();
        }

        public void FindMember_()
        {
            throw new NotImplementedException();
        }

        public void FindMethod_()
        {
            throw new NotImplementedException();
        }

        public void FindField_()
        {
            throw new NotImplementedException();
        }

        public void FindMemberRef_()
        {
            throw new NotImplementedException();
        }

        public void GetMethodProps([In] uint md, [Out] out int pClass, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder szMethod, [In] int cchMethod, [Out] out int pchMethod, [Out] out uint pdwAttr, [Out] out IntPtr ppvSigBlob, [Out] out uint pcbSigBlob, [Out] out uint pulCodeRVA, [Out] out uint pdwImplFlags)
        {
            throw new NotImplementedException();
        }

        public void GetMemberRefProps([In] uint mr, [Out] out int ptk, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder szMember, [In] int cchMember, [Out] out uint pchMember, [Out] out IntPtr ppvSigBlob, [Out] out int pbSig)
        {
            throw new NotImplementedException();
        }

        public void EnumProperties(ref IntPtr phEnum, int mdTypeDef, out int mdPropertyDef, int countMax, out uint pcTokens)
        {
            throw new NotImplementedException();
        }

        public void EnumEvents_()
        {
            throw new NotImplementedException();
        }

        public void GetEventProps_()
        {
            throw new NotImplementedException();
        }

        public void EnumMethodSemantics_()
        {
            throw new NotImplementedException();
        }

        public void GetMethodSemantics_()
        {
            throw new NotImplementedException();
        }

        public void GetClassLayout_()
        {
            throw new NotImplementedException();
        }

        public void GetFieldMarshal_()
        {
            throw new NotImplementedException();
        }

        public void GetRVA_()
        {
            throw new NotImplementedException();
        }

        public void GetPermissionSetProps_()
        {
            throw new NotImplementedException();
        }

        public void GetSigFromToken_()
        {
            throw new NotImplementedException();
        }

        public void GetModuleRefProps_()
        {
            throw new NotImplementedException();
        }

        public void EnumModuleRefs_()
        {
            throw new NotImplementedException();
        }

        public void GetTypeSpecFromToken_()
        {
            throw new NotImplementedException();
        }

        public void GetNameFromToken_()
        {
            throw new NotImplementedException();
        }

        public void EnumUnresolvedMethods_()
        {
            throw new NotImplementedException();
        }

        public void GetUserString([In] int stk, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder szString, [In] int cchString, out int pchString)
        {
            throw new NotImplementedException();
        }

        public void GetPinvokeMap_()
        {
            throw new NotImplementedException();
        }

        public void EnumSignatures_()
        {
            throw new NotImplementedException();
        }

        public void EnumTypeSpecs_()
        {
            throw new NotImplementedException();
        }

        public void EnumUserStrings_()
        {
            throw new NotImplementedException();
        }

        public void GetParamForMethodIndex_()
        {
            throw new NotImplementedException();
        }

        public void EnumCustomAttributes(ref IntPtr phEnum, int tk, int tkType, out int mdCustomAttribute, uint cMax, out uint pcTokens)
        {
            throw new NotImplementedException();
        }

        public void GetCustomAttributeProps_()
        {
            throw new NotImplementedException();
        }

        public void FindTypeRef_()
        {
            throw new NotImplementedException();
        }

        public void GetMemberProps_()
        {
            throw new NotImplementedException();
        }

        public void GetFieldProps(int mb, out int mdTypeDef, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder szField, int cchField, out int pchField, out int pdwAttr, out IntPtr ppvSigBlob, out int pcbSigBlob, out int pdwCPlusTypeFlab, out IntPtr ppValue, out int pcchValue)
        {
            throw new NotImplementedException();
        }

        public void GetPropertyProps_()
        {
            throw new NotImplementedException();
        }

        public void GetParamProps(int tk, out int pmd, out uint pulSequence, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder szName, uint cchName, out uint pchName, out uint pdwAttr, out uint pdwCPlusTypeFlag, out IntPtr ppValue, out uint pcchValue)
        {
            throw new NotImplementedException();
        }

        public int GetCustomAttributeByName(int tkObj, [MarshalAs(UnmanagedType.LPWStr)] string szName, out IntPtr ppData, out uint pcbData)
        {
            throw new NotImplementedException();
        }

        public void GetNestedClassProps(int tdNestedClass, out int tdEnclosingClass)
        {
            throw new NotImplementedException();
        }

        public void GetNativeCallConvFromSig_()
        {
            throw new NotImplementedException();
        }

        public void IsGlobal_()
        {
            throw new NotImplementedException();
        }

        public bool IsValidToken([In, MarshalAs(UnmanagedType.U4)] uint tk)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
