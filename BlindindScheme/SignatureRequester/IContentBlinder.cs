using System;
using System.Collections.Generic;
using System.Text;

namespace BlindindScheme.SignatureRequester
{
    public interface IContentBlinder
    {
        /// <summary>
        /// Get blinded content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        byte[] GetBlindedContent(byte[] content);

        byte[] GetUnblindedSignature(byte[] blindedSignature);
    }
}
