using System;
using System.Collections.Generic;
using System.Text;

namespace BlindindScheme.SignatureRequester
{
    public class SignedEntity : ISignedEntity
    {
        private readonly byte[] content;
        private readonly byte[] signature;

        public SignedEntity(byte[] content, byte[] signature)
        {
            this.content = content;
            this.signature = signature;
        }
        public byte[] GetSignature()
        {
            return signature;
        }

        public byte[] GetSignedContent()
        {
            return content;
        }
    }
}
