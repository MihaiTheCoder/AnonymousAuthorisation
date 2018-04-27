using BlindindScheme;
using BlindindScheme.SignatureRequester;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlindChatCore.Model
{
    public class ClientParticipant
    {
        private readonly IAPIServer server;

        private readonly IGroupRepository groupRepository;

        public ClientParticipant(IAPIServer server, IGroupRepository groupRepository)
        {
            this.server = server;
            this.groupRepository = groupRepository;
        }

        public GroupRegistration GetGroupRegistration(int invitationCode, RsaKeyParameters participantPublicKey)
        {
            var group = server.GetGroup(invitationCode);
            return new GroupRegistration(group, new ContentBlinder(group.RsaPublicKey), participantPublicKey);
        }

        public RsaKeyParameters GetGroupDetails(int invitationCode)
        {
            var participant = groupRepository.GetParticipant(invitationCode);
            var group = groupRepository.GetGroup((Guid)participant.GroupId);

            return group.RsaPublicKey;
        }

        public void RegisterBlindCertificate(int invitationCode, GroupRegistration groupRegistration)
        {
            server.RegisterBlindCertificate(GetBlindedPublickey(groupRegistration.ContentBlinder, groupRegistration.PublicKey), invitationCode);            
        }

        public VerifiedParticipant CheckVerifiedEntity(Group group, string email, GroupRegistration groupRegistration)
        {
            var signedMessage = server.GetSignedMessage(group.Id, email);

            if (signedMessage == null)
                return null;

            VerifiedParticipant verifiedParticipant = new VerifiedParticipant();
            verifiedParticipant.PublicKey = RsaKeyUtils.GetSerializedPublicKey(groupRegistration.PublicKey);
            verifiedParticipant.Signature = GetSignature(groupRegistration, signedMessage);


            return verifiedParticipant;
        }

        public void AddClientCertificate(VerifiedParticipant verifiedParticipant, Group group, string email)
        {
            groupRepository.SaveClientCertificate(verifiedParticipant, group.Id, email);
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

        public List<Participant> GetUnconfirmedParticipants()
        {
            return server.GetParticipantsToConfirm();
        }

        public void AddBlindParticipant(Guid groupId, VerifiedParticipant verifiedParticipant)
        {
            server.AddNewBlindParticipant(groupId, verifiedParticipant.PublicKey, verifiedParticipant.Signature);
        }
    }
}
