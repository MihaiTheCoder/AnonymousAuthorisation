using BlindindScheme.SignatureRequester;
using Org.BouncyCastle.Crypto.Parameters;

namespace BlindChatCore
{
    public class GroupRegistration
    {
        public GroupRegistration(Group group, IContentBlinder contentBlinder, RsaKeyParameters participantPublicKey)
        {
            Group = group;
            ContentBlinder = contentBlinder;
            ParticipantPublicKey = participantPublicKey;
        }

        public Group Group { get; }
        public IContentBlinder ContentBlinder { get; }
        public RsaKeyParameters ParticipantPublicKey { get; }
    }
}