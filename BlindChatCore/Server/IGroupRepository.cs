using System;
using System.Collections.Generic;

namespace BlindChatCore
{
    public interface IGroupRepository
    {
        void SaveGroup(Group group);
        void AddConfirmationCode(Guid id, int confirmationCode);
        Guid? GetGroupIdForConfirmationCode(int confirmationCode);
        void RemoveConfirmationCode(int confirmationCode);
        void AddParticipant(string email, int invitationCode);
        void GetGroupOwnerEmail(Guid groupId);
        Group GetGroup(Guid groupId);
        List<Participant> GetParticipants();
        Participant GetParticipant(int invitationCode);
        void SetBlindedCertificate(int iD, string blindedCertificate);
    }
}