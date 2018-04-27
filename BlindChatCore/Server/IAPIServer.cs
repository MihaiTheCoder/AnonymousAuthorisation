using System;
using System.Collections.Generic;

namespace BlindChatCore.Model
{
    public interface IAPIServer
    {
        void CreateGroup(Group group);

        void SendConfirmationCodeToGroupCreator(Group group);

        List<ParticipantStatus> AddParticipantsToGroup(List<string> emails, int confirmationCode);

        void RegisterBlindCertificate(string blindedCertificate, int invitationCode);

        List<MessageToSign> GetMessagesToSign(Guid groupId);

        void SaveSignedMessages(Guid groupId, List<SignedMessage> signedMessages);
        Group GetGroup(int invitationCode);
        SignedMessage GetSignedMessage(Guid groupId, string email);
        List<Participant> GetParticipantsToConfirm();

        void AddMessage(Guid groupId, ParticipantMessage message, VerifiedParticipant participant);
        void AddNewBlindParticipant(Guid groupId, string publicKey, string signature);
        List<VerifiedParticipant> GetBlindParticipants(Guid groupId);
    }
}