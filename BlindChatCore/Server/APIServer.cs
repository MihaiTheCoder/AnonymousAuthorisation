using BlindChatCore.Certificate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BlindChatCore
{
    public class APIServer : IAPIServer
    {
        private readonly IGroupRepository groupRepository;
        private readonly IEmailSender emailSender;
        private readonly ISimpleRandomGenerator simpleRandomGenerator;

        public APIServer(IGroupRepository groupRepository, IEmailSender emailSender, ISimpleRandomGenerator simpleRandomGenerator)
        {
            this.groupRepository = groupRepository;
            this.emailSender = emailSender;
            this.simpleRandomGenerator = simpleRandomGenerator;
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
                groupRepository.AddParticipant(email, invitationCode);
                emailSender.SendInvitation(email, invitationCode);
                participants.Add(new ParticipantStatus(email, true));
            }
            foreach (var email in emails.Where(e => emailsAlreadyInvited.Contains(e)))
            {
                participants.Add(new ParticipantStatus(email, false));
            }
            return participants;
        }

        public void CreateGroup(Group group)
        {
            groupRepository.SaveGroup(group);
            SendConfirmationCode(group);
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

            groupRepository.SetBlindedCertificate(participant.ID, blindedCertificate);

            //notify Group Owner
        }
    }
}
