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
                EmailIsAlreadyInvited = true,
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
            var certificates = from auth in context.AuthenticationMessages where auth.GroupId == groupId && auth.IsSigned == null select auth;          
            var messageToSign = new MessageToSign();
            foreach (var certificate in certificates)
            {
                messageToSign.Message = certificate.Message;
                var participant = context.Participants.FirstOrDefault(p => p.Id == certificate.ParticipantId);
                messageToSign.Email = participant.Email;
                list.Add(messageToSign);
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
            var participant = context.Participants.FirstOrDefault(p => p.Email == email);
            var authMessage = context.AuthenticationMessages.FirstOrDefault(m => m.GroupId == groupId && m.ParticipantId == participant.Id);
            var signedMessage = new SignedMessage();

            signedMessage.Email = participant.Email;
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

            var participant = context.Participants.FirstOrDefault(p => p.GroupId == groupId && p.Email == email);
            
            if (participant != null)
            {
                participant.EmailIsConfirmed = true;              
            }
            context.Participants.Update(participant);
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
            var blindParticipant = context.BlindParticipants.FirstOrDefault(p => p.PublicKey == participant.PublicKey && p.Signature == participant.Signature);
            var conversationMessage = new ConversationMessage
            {
                GroupId = blindParticipant.GroupId,
                Message = message.Message
            };
            context.ConversationMessages.Add(conversationMessage);
            context.SaveChanges();
        }

        public void SaveSignedCertificates(Guid groupId, List<SignedMessage> signedMessages)
        {
            foreach (var message in signedMessages)
            {
                var participant = context.Participants.FirstOrDefault(p => p.Email == message.Email);
                var authMessage = context.AuthenticationMessages.FirstOrDefault(a => a.GroupId == groupId && a.ParticipantId == participant.Id);
                authMessage.IsSigned = true;
                authMessage.Signature = message.Signature;
                context.AuthenticationMessages.Update(authMessage);
            }            
            context.SaveChanges();
        }

        public void AddBlindedCertificate(int participantId, Guid? groupId, string blindedCertificate)
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

        public bool HasBlindCertificate(int invitationCode)
        {
            var participant = context.Participants.FirstOrDefault(p => p.InvitationCode == invitationCode);
            var authenticateMessage = context.AuthenticationMessages.FirstOrDefault(a => a.ParticipantId == participant.Id);
            
            if (authenticateMessage.Message != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Participant> UnconfirmedParticipants()
        {
            List<Participant> list = new List<Participant>();
            var participants = context.Participants.Where(p => p.EmailIsConfirmed == false);
            foreach (var participant in participants)
            {
                list.Add(participant);
            }
            return list;
        }

        public void InsertBlindParticipant(Guid groupId, string publicKey, string signature)
        {
            var blindParticipant = new BlindParticipant
            {
                GroupId = groupId,
                PublicKey = publicKey,
                Signature = signature
            };
            context.BlindParticipants.Add(blindParticipant);
            context.SaveChanges();
        }

        public List<VerifiedParticipant> GetBlindParticipants(Guid groupId)
        {
            List<VerifiedParticipant> list = new List<VerifiedParticipant>();
            VerifiedParticipant verifiedParticipant = new VerifiedParticipant();
            var blindparticipants = context.BlindParticipants.Where(b => b.GroupId == groupId).ToList();
            foreach(var blindparticipant in blindparticipants)
            {
                verifiedParticipant.PublicKey = blindparticipant.PublicKey;
                verifiedParticipant.Signature = blindparticipant.Signature;
                list.Add(verifiedParticipant);
            }
            return list;
        }
    }
}
