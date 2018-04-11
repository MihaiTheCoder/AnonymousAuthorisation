using System;
using System.Collections.Generic;
using System.Text;

namespace BlindChatCore.Server
{
    public class GroupRepository : IGroupRepository
    {
        public GroupRepository()
        {

        }

        public void AddConfirmationCode(Guid id, int confirmationCode)
        {
            throw new NotImplementedException();
        }

        public void AddParticipant(string email, int invitationCode)
        {
            throw new NotImplementedException();
        }

        public List<MessageToSign> GetBlindCertificatesToSign(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Group GetGroup(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Group GetGroupForInvitationCode(int invitationCode)
        {
            throw new NotImplementedException();
        }

        public Guid? GetGroupIdForConfirmationCode(int confirmationCode)
        {
            throw new NotImplementedException();
        }

        public void GetGroupOwnerEmail(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Participant GetParticipant(int invitationCode)
        {
            throw new NotImplementedException();
        }

        public List<Participant> GetParticipants()
        {
            throw new NotImplementedException();
        }

        public SignedMessage GetSignedMessage(Guid groupId, string email)
        {
            throw new NotImplementedException();
        }

        public void MarkParticipantEmailUsed(int iD)
        {
            throw new NotImplementedException();
        }

        public void RemoveConfirmationCode(int confirmationCode)
        {
            throw new NotImplementedException();
        }

        public void SaveGroup(Group group)
        {
            throw new NotImplementedException();
        }

        public void SaveMessage(VerifiedParticipant participant, ParticipantMessage message)
        {
            throw new NotImplementedException();
        }

        public void SaveSignedCertificates(Guid groupId, List<SignedMessage> signedMessages)
        {
            throw new NotImplementedException();
        }

        public void SetBlindedCertificate(int participantId, Guid groupId, string blindedCertificate)
        {
            throw new NotImplementedException();
        }
    }
}
