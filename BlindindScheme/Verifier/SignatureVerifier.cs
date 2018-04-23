using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlindindScheme
{
    public class SignatureVerifier : ISignatureVerifier
    {
        public bool Verify(ISignedEntity signedEntity, AsymmetricKeyParameter publicKey)
        {
            byte[] content = signedEntity.GetSignedContent();

            byte[] signature = signedEntity.GetSignature();

            PssSigner signer = new PssSigner(new RsaEngine(), new Sha1Digest(), 20);
            signer.Init(false, publicKey);

            signer.BlockUpdate(content, 0, content.Length);

            return signer.VerifySignature(signature);
        }
    }
}
