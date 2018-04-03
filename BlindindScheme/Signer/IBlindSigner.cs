using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlindindScheme
{
    public interface IBlindSigner
    {
        /// <summary>
        /// The Signer's RSA public Key
        /// </summary>
        /// <returns></returns>
        RsaKeyParameters GetPublic();

        byte[] Sign(byte[] blindedContent);
    }
}
