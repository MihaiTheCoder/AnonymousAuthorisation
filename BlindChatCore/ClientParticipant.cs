using BlindindScheme;
using BlindindScheme.SignatureRequester;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlindChatCore
{
    public class ClientParticipant
    {
        private readonly IAPIServer server;

        public ClientParticipant(IAPIServer server)
        {
            this.server = server;
        }

        public GroupRegistration GetGroupRegistration(int invitationCode, RsaKeyParameters participantPublicKey)
        {
            var group = server.GetGroup(invitationCode);
            return new GroupRegistration(group, new ContentBlinder(group.RsaPublicKey), participantPublicKey);
        }

        public void RegisterBlindCertificate(int invitationCode, GroupRegistration groupRegistration)
        {
            server.RegisterBlindCertificate(GetBlindedPublickey(groupRegistration.ContentBlinder, groupRegistration.ParticipantPublicKey), invitationCode);            
        }

        public VerifiedParticipant CheckVerifiedEntity(Group group, string email, GroupRegistration groupRegistration)
        {
            var signedMessage = server.GetSignedMessage(group.Id, email);

            if (signedMessage == null)
                return null;

            VerifiedParticipant verifiedParticipant = new VerifiedParticipant();
            verifiedParticipant.PublicKey = RsaKeyUtils.GetSerializedPublicKey(groupRegistration.ParticipantPublicKey);
            verifiedParticipant.Signature = GetSignature(groupRegistration, signedMessage);
            return verifiedParticipant;
        }

        public void AddMessage(Guid groupId, ParticipantMessage message, VerifiedParticipant participant)
        {
            server.AddMessage(groupId, message, participant);
        }

        private static string GetSignature(GroupRegistration groupRegistration, SignedMessage signedMessage)
        {
            return Convert.ToBase64String(groupRegistration.ContentBlinder.GetUnblindedSignature(Convert.FromBase64String(signedMessage.Signature)));
        }

        private string GetBlindedPublickey(IContentBlinder contentBlinder, RsaKeyParameters participantPublicKey)
        {
            byte[] message = Convert.FromBase64String(RsaKeyUtils.GetSerializedPublicKey(participantPublicKey));
            byte[] blindedMessage = contentBlinder.GetBlindedContent(message);
            return Convert.ToBase64String(blindedMessage);
        }
    }
}
