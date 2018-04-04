using System;
using System.Collections.Generic;

namespace BlindChatCore
{
    public interface IAPIServer
    {
        void CreateGroup(Group group);

        void SendConfirmationCodeToGroupCreator(Group group);

        List<ParticipantStatus> AddParticipantsToGroup(List<string> emails, int confirmationCode);

        void RegisterBlindCertificate(string blindedCertificate, int invitationCode);
        
    }
}