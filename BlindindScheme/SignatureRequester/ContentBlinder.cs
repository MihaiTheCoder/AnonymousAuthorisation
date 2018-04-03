using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlindindScheme.SignatureRequester
{
    public class ContentBlinder : IContentBlinder
    {
        private readonly RsaBlindingParameters blindingParams;

        public ContentBlinder(RsaKeyParameters publicKey)
        {
            RsaBlindingFactorGenerator blindingFactorGenerator = new RsaBlindingFactorGenerator();
            blindingFactorGenerator.Init(publicKey);
            BigInteger blindingFactor = blindingFactorGenerator.GenerateBlindingFactor();
            blindingParams = new RsaBlindingParameters(publicKey, blindingFactor);            
        }

        public byte[] GetBlindedContent(byte[] content)
        {
            PssSigner signer = new PssSigner(new RsaBlindingEngine(), new Sha1Digest(), 20);
            signer.Init(true, blindingParams);
            signer.BlockUpdate(content, 0, content.Length);
            byte[] sig = signer.GenerateSignature();
            return sig;
        }

        public byte[] GetUnblindedSignature(byte[] blindedSignature)
        {
            RsaBlindingEngine blindingEngine = new RsaBlindingEngine();
            blindingEngine.Init(false, blindingParams);
            byte[] unblindedSignature = blindingEngine.ProcessBlock(blindedSignature, 0, blindedSignature.Length);
            return unblindedSignature;
        }
    }
}
