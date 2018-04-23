using BlindindScheme.SignatureRequester;
using Org.BouncyCastle.Crypto.Parameters;

namespace BlindChatCore.Model
{
    public class GroupRegistration
    {
        public GroupRegistration(Group group, IContentBlinder contentBlinder, RsaKeyParameters publicKey)
        {
            Group = group;
            ContentBlinder = contentBlinder;
            PublicKey = publicKey;
        }

        public Group Group { get; }
        public IContentBlinder ContentBlinder { get; }
        public RsaKeyParameters PublicKey { get; }
    }
}