using Org.BouncyCastle.Crypto.Parameters;

namespace BlindChatCore
{
    public class VerifiedParticipant
    {
        public string PublicKey { get; set; }

        public string Signature { get; set; }
    }
}