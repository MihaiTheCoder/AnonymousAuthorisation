using System;
using System.Collections.Generic;
using System.Text;

namespace BlindindScheme
{
    public interface ISignedEntity
    {
        /// <summary>
        /// The content that was signed
        /// </summary>
        /// <returns></returns>
        byte[] GetSignedContent();


        /// <summary>
        /// The signature of the blind signer
        /// </summary>
        /// <returns></returns>
        byte[] GetSignature();
    }
}
