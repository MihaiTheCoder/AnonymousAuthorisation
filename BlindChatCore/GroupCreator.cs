using BlindindScheme;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlindChatCore.Model
{
    public class GroupCreator
    {
        private readonly IAPIServer server;
        private readonly IBlindSigner blindSigner;

        public GroupCreator(IAPIServer server, IBlindSigner blindSigner)
        {
            this.server = server;
            this.blindSigner = blindSigner;
        }

        public void RegisterGroup(Group group)
        {
            server.CreateGroup(group);
        }

        public void AddParticipantsToGroup(List<string> emails, int confirmationCode)
        {
            server.AddParticipantsToGroup(emails, confirmationCode);
        }

        public void SignMessages(Group group)
        {
            var messagesToSign = server.GetMessagesToSign(group.Id);
            List<SignedMessage> signedMessage = new List<SignedMessage>(messagesToSign.Count);
            foreach (var message in messagesToSign)
            {
                signedMessage.Add(new SignedMessage { Email = message.Email, Message = message.Message, Signature = GetSignature(message) });
            }
            server.SaveSignedMessages(group.Id, signedMessage);
        }

        private string GetSignature(MessageToSign message)
        {
            return ToBase64String(blindSigner.Sign(FromBase64String(message.Message)));
        }

        byte[] FromBase64String(string message)
        {
            return Convert.FromBase64CharArray(message.ToCharArray(), 0, message.Length);
        }

        string ToBase64String(byte[] message)
        {
            return Convert.ToBase64String(message);
        }

        public List<VerifiedParticipant> GetBlindParticipants(Guid groupId)
        {
            return server.GetBlindParticipants(groupId);
        }

        public List<Participant> UnconfirmedInvitedParticipants(string groupName)
        {
            return server.GetParticipantsToConfirm(groupName);
        }

        public Participant GetParticipantToConfirm(string groupName, string participantEmail)
        {
            return server.GetParticipantToConfirm(groupName, participantEmail);
        }

        public Group GetGroup(int invitationCode)
        {
            return server.GetGroup(invitationCode);
        }

        public BlindParticipant GetBlindParticipantByNickname(Guid id, string nickname)
        {
            return server.GetBlindParticipantByContent(id, nickname);
        }
    }
}
