using BlindChatCore.Certificate;
using BlindChatCore.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BlindindScheme.SignatureRequester;
using BlindindScheme;

namespace BlindChatCore.Model
{
    public class APIServer : IAPIServer
    {
        private readonly IGroupRepository groupRepository;
        private readonly IEmailSender emailSender;
        private readonly ISimpleRandomGenerator simpleRandomGenerator;
        private readonly ISignatureVerifier signatureVerifier;
        public Dictionary<int, Tuple<Guid?, string>> groupDetailDictionary = new Dictionary<int, Tuple<Guid?, string>>();

        public APIServer(IGroupRepository groupRepository, IEmailSender emailSender, ISimpleRandomGenerator simpleRandomGenerator, ISignatureVerifier signatureVerifier)
        {
            this.groupRepository = groupRepository;
            this.emailSender = emailSender;
            this.simpleRandomGenerator = simpleRandomGenerator;
            this.signatureVerifier = signatureVerifier;
        }
        public List<ParticipantStatus> AddParticipantsToGroup(List<string> emails, int confirmationCode)
        {
            Guid? groupId = groupRepository.GetGroupIdForConfirmationCode(confirmationCode);

            if (!groupId.HasValue)
                throw new ArgumentException("confirmationCode invalid");

            groupRepository.RemoveConfirmationCode(confirmationCode);

            string[] emailsAlreadyInvited = groupRepository.GetParticipants().Select(p => p.Email).ToArray();

            List<ParticipantStatus> participants = new List<ParticipantStatus>();

            foreach (var email in emails.Where(e => !emailsAlreadyInvited.Contains(e)))
            {
                int invitationCode = GenerateConfirmationCode();
                groupRepository.AddParticipant(email, invitationCode, groupId);
                emailSender.SendInvitation(email, invitationCode);
                participants.Add(new ParticipantStatus(email, true));
                var tupleGE = new Tuple<Guid?, string>(groupId, email);
                groupDetailDictionary.Add(invitationCode, tupleGE);
            }
            foreach (var email in emails.Where(e => emailsAlreadyInvited.Contains(e)))
            {
                participants.Add(new ParticipantStatus(email, false));
            }
            return participants;
        }

        public void CreateGroup(Group group)
        {
            var newGroup = groupRepository.SaveGroup(group);
            SendConfirmationCode(newGroup);
        }

        private int GenerateConfirmationCode()
        {
            return simpleRandomGenerator.Next((int)Math.Pow(10, 5), ((int)Math.Pow(10, 6) - 1));
        }

        public void SendConfirmationCodeToGroupCreator(Group inputGroup)
        {
            Group group = groupRepository.GetGroup(inputGroup.Id);

            if (group == null)
                throw new ArgumentException("inputGroup");

            if (group.OwnerEmail != inputGroup.OwnerEmail)
                throw new ArgumentException("inputGroup");

            SendConfirmationCode(group);
        }

        private void SendConfirmationCode(Group group)
        {
            var confirmationCode = GenerateConfirmationCode();
            groupRepository.AddConfirmationCode(group.Id, confirmationCode);
            emailSender.SendConfirmationCodeToGroupCreator(group.OwnerEmail, confirmationCode);
        }

        public void RegisterBlindCertificate(string blindedCertificate, int invitationCode)
        {
            Participant participant = groupRepository.GetParticipant(invitationCode);

            groupRepository.SetBlindedCertificate(participant.Id, participant.GroupId, blindedCertificate);            

            groupRepository.MarkParticipantEmailUsed(participant.Id);
        }

        public List<MessageToSign> GetMessagesToSign(Guid groupId)
        {
            return groupRepository.GetBlindCertificatesToSign(groupId);
        }

        public void SaveSignedMessages(Guid groupId, List<SignedMessage> signedMessages)
        {
            groupRepository.SaveSignedCertificates(groupId, signedMessages);
        }

        public SignedMessage GetSignedMessage(Guid groupId, string email)
        {
            return groupRepository.GetSignedMessage(groupId, email);
        }

        public void AddMessage(Guid groupId, ParticipantMessage message, VerifiedParticipant participant)
        {
            var groupDetails = groupRepository.GetGroup(groupId);

            SignedEntity signedEntity = new SignedEntity(FromBase64String(participant.PublicKey), FromBase64String(participant.Signature));

            bool isVerified = signatureVerifier.Verify(signedEntity, groupDetails.RsaPublicKey);

            if (isVerified)
                groupRepository.SaveMessage(participant, message);
        }

        byte[] FromBase64String(string message)
        {
            return Convert.FromBase64CharArray(message.ToCharArray(), 0, message.Length);            
        }

        public Group GetGroup(int invitationCode)
        {
            Group group = groupRepository.GetGroupForInvitationCode(invitationCode);
            return group;
        }
    }
}
