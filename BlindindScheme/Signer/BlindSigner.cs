using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace BlindindScheme
{
    public class BlindSigner : IBlindSigner
    {
        private readonly AsymmetricCipherKeyPair keys;

        public BlindSigner(AsymmetricCipherKeyPair keys)
        {
            this.keys = keys;
        }
        public RsaKeyParameters GetPublic()
        {
            return (RsaKeyParameters)keys.Public;
        }

        public byte[] Sign(byte[] blindedContent)
        {
            RsaEngine engine = new RsaEngine();
            engine.Init(true, keys.Private);
            return engine.ProcessBlock(blindedContent, 0, blindedContent.Length);
        }
    }
}
