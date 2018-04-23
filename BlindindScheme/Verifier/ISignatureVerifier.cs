using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlindindScheme
{
    public interface ISignatureVerifier
    {
        bool Verify(ISignedEntity signedEntity, AsymmetricKeyParameter publicKey);
    }
}
