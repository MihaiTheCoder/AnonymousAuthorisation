using BlindChatCore.Model;
using BlindChat.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlindChatCore.Model
{


    public class GroupRepository : IGroupRepository
    {
        private readonly BlindChatDbContext context;

        public GroupRepository(BlindChatDbContext dbContext)
        {
            this.context = dbContext;
        }

        public void AddConfirmationCode(Guid groupId, int confirmationCode)
        {
            var confirmCode = new ConfirmationCode { Code = confirmationCode, GroupId = groupId, IsDeleted = false};
            context.ConfirmationCodes.Add(confirmCode);
            context.SaveChanges();
        }

        public void AddParticipant(string email, int invitationCode, Guid? groupId)
        {
            var participant = new BlindChatCore.Model.Participant {
                Email = email,
                EmailIsAlreadyInvited = false,
                EmailIsConfirmed = false,
                InvitationCode = invitationCode,
                GroupId = groupId
            };
            context.Participants.Add(participant);
            context.SaveChanges();
        }

        public List<MessageToSign> GetBlindCertificatesToSign(Guid groupId)
        {
            List<MessageToSign> list = new List<MessageToSign>();
            var certificates = context.AuthenticationMessages.Where(c => c.GroupId == groupId && c.IsSigned == false);
            var messageToSign = new MessageToSign();
            foreach (var certificate in certificates)
            {
                messageToSign.Message = certificate.Message;
                messageToSign.Email = certificate.Group.OwnerEmail;
                list.Append(messageToSign);
            }
            return list;
        }

        public Group GetGroup(Guid groupId)
        {
            var group = context.Groups.FirstOrDefault(g => g.Id == groupId);

            return group;
        }

        public Group GetGroupForInvitationCode(int invitationCode)
        {
            var participant = context.Participants.FirstOrDefault(p => p.InvitationCode == invitationCode);
            var group = context.Groups.FirstOrDefault(g => g.Id == participant.GroupId);
            
            return group;
        }

        public Guid? GetGroupIdForConfirmationCode(int confirmationCode)
        {
            var confCode = context.ConfirmationCodes.FirstOrDefault(c => c.Code == confirmationCode);

            return confCode.GroupId;
        }

        public void GetGroupOwnerEmail(Guid groupId)
        {
            var owner = context.Groups.FirstOrDefault(o => o.Id == groupId);
        }

        public Model.Participant GetParticipant(int invitationCode)
        {
            var participant = context.Participants.FirstOrDefault(p => p.InvitationCode == invitationCode);

            return participant;
        }

        public List<Participant> GetParticipants()
        {
            List<Participant> list = new List<Participant>();
            var participants = context.Participants;
            foreach (var participant in participants)
            {

                list.Append(participant);
            }
            
            return list;
        }

        public SignedMessage GetSignedMessage(Guid groupId, string email)
        {
            var authMessage = context.AuthenticationMessages.FirstOrDefault(m => m.GroupId == groupId && m.Group.OwnerEmail == email);
            var signedMessage = new SignedMessage();

            signedMessage.Email = authMessage.Group.OwnerEmail;
            signedMessage.Message = authMessage.Message;
            signedMessage.Signature = authMessage.Signature;

            return signedMessage;
        }

        public void MarkParticipantEmailUsed(int participantId)
        {
            var participant = context.Participants.FirstOrDefault(p => p.Id == participantId);
            if (participant != null)
            {
                participant.EmailIsConfirmed = true;
                context.SaveChanges();
            }
        }

        public void RemoveConfirmationCode(int confirmationCode)
        {
            var confirmCode = context.ConfirmationCodes.FirstOrDefault(c => c.Code == confirmationCode);
            if (confirmCode != null)
            {
                confirmCode.IsDeleted = true;
                context.SaveChanges();
            }
        }

        public void SaveClientCertificate(VerifiedParticipant verifiedParticipant, Guid groupId, string email)
        {
            var participant = new Participant
            {
                GroupId = groupId,
                Email = email,
                EmailIsAlreadyInvited = true,
                EmailIsConfirmed = true,
                PublicKey = verifiedParticipant.PublicKey,
                Signature = verifiedParticipant.Signature
            };
            context.Participants.Add(participant);
            context.SaveChanges();
        }

        public Group SaveGroup(Group group)
        {
            var newGroup = new Group
            {
                Id = Guid.NewGuid(),
                Name = group.Name,
                OwnerEmail = group.OwnerEmail,
                RsaPublicKey = group.RsaPublicKey
            };
            context.Groups.Add(newGroup);
            context.SaveChanges();

            return newGroup;
        }

        public void SaveMessage(VerifiedParticipant participant, ParticipantMessage message)
        {
            var participantdb = context.Participants.FirstOrDefault(p => p.PublicKey == participant.PublicKey && p.Signature == participant.Signature);
            var conversationMessage = new ConversationMessage
            {
                GroupId = participantdb.GroupId,
                Message = message.Message,
                ParticipantId = participantdb.Id
            };
            context.ConversationMessages.Add(conversationMessage);
            context.SaveChanges();
        }

        public void SaveSignedCertificates(Guid groupId, List<SignedMessage> signedMessages)
        {
            var authMessage = new AuthenticationMessage();
            foreach (var message in signedMessages)
            {
                authMessage.GroupId = groupId;
                authMessage.Message = message.Message;
                authMessage.Signature = message.Signature;
            }
            context.AuthenticationMessages.Add(authMessage);
            context.SaveChanges();
        }

        public void SetBlindedCertificate(int participantId, Guid? groupId, string blindedCertificate)
        {
            var blindCertificate = new AuthenticationMessage
            {
                Message = blindedCertificate,
                GroupId = groupId,
                ParticipantId = participantId,
            };
            context.AuthenticationMessages.Add(blindCertificate);
            context.SaveChanges();
        }
    }
}
