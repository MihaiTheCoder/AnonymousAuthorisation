using System;
using System.Collections.Generic;

namespace BlindChatCore.Model
{
    public interface IGroupRepository
    {
        Group SaveGroup(Group group);
        void AddConfirmationCode(Guid groupId, int confirmationCode);
        Guid? GetGroupIdForConfirmationCode(int confirmationCode);
        void RemoveConfirmationCode(int confirmationCode);
        void AddParticipant(string email, int invitationCode, Guid? groupId);
        void GetGroupOwnerEmail(Guid groupId);
        Group GetGroup(Guid groupId);
        List<Participant> GetParticipants();
        Participant GetParticipant(int invitationCode);
        void SetBlindedCertificate(int participantId, Guid? groupId, string blindedCertificate);
        void MarkParticipantEmailUsed(int participantId);
        List<MessageToSign> GetBlindCertificatesToSign(Guid groupId);
        void SaveSignedCertificates(Guid groupId, List<SignedMessage> signedMessages);
        SignedMessage GetSignedMessage(Guid groupId, string email);
        void SaveMessage(VerifiedParticipant participant, ParticipantMessage message);
        Group GetGroupForInvitationCode(int invitationCode);
        void SaveClientCertificate(VerifiedParticipant verifiedParticipant, Guid groupId, string email);
    }
}