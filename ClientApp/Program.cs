using System;
using System.Collections.Generic;
using BlindChat.Infrastructure.Repository;
using BlindChatCore;
using BlindChatCore.Certificate;
using BlindChatCore.Model;
using BlindChatCore.Server;
using BlindindScheme;
using BlindindScheme.SignatureRequester;
using Org.BouncyCastle.Crypto.Parameters;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Asta va fi inlocuit cu un API call
            SignatureVerifier signatureVerifier = new SignatureVerifier();
            RNGRandomGenerator rngGenerator = new RNGRandomGenerator();
            EmailSender emailSender = new EmailSender();
            BlindChatDbContext context = new BlindChatDbContext();
            GroupRepository groupRepository = new GroupRepository(context);
            APIServer server = new APIServer(groupRepository, emailSender, rngGenerator, signatureVerifier);

            //Set participants
            List<Participant> unconfirmedParticipants = server.GetParticipantsToConfirm();
            foreach(var participant in unconfirmedParticipants)
            {
                int invitationCode = participant.InvitationCode;
                Guid groupId = (Guid)participant.GroupId;
                string email = participant.Email;
                Group group = server.GetGroup(participant.InvitationCode);

                ClientParticipant clientParticipant = new ClientParticipant(server, groupRepository);
                var groupPublicKey = clientParticipant.GetGroupDetails(invitationCode);

                //Generate certificate
                CertificateGenerator generator = new CertificateGenerator();
                var participantKeys = generator.GenerateCertificate("C=RO,O=Qubiz", TimeSpan.FromDays(1), "certParticipant.pfx", "Test.123");

                //Serialize 
                var privateSerializedKey = RsaKeyUtils.GetSerializedPrivateKey(participantKeys.Private);
                var publicSerializedKey = RsaKeyUtils.GetSerializedPublicKey(participantKeys.Public);

                //Concatenante serialized key
                var content = RsaKeyUtils.Combine(publicSerializedKey, privateSerializedKey);               

                //Generate blind content
                ContentBlinder contentBlinder = new ContentBlinder((RsaKeyParameters)groupPublicKey);
                var blindedContent = contentBlinder.GetBlindedContent(content);
                var groupRegistration = clientParticipant.GetGroupRegistration(invitationCode,(RsaKeyParameters)participantKeys.Public);

                //Save blindedCertificate 
                clientParticipant.RegisterBlindCertificate(invitationCode, groupRegistration);                

                //Send for sign DONE

                //Get blindSignature
                var blindMessage = server.GetSignedMessage(groupId,email);
                var signature = Convert.FromBase64CharArray(blindMessage.Signature.ToCharArray(), 0, blindMessage.Signature.Length);

                //Unblind signature
                var unblindedSignature = contentBlinder.GetUnblindedSignature(signature);

                //Verify
                var verifiedParticipant = clientParticipant.CheckVerifiedEntity(group, participant.Email, groupRegistration);
                clientParticipant.AddClientCertificate(verifiedParticipant, group, email);
                ParticipantMessage message = new ParticipantMessage();
                message.Message = "Andreiu, ce nevoie faci?";
                clientParticipant.AddMessage(groupId, message, verifiedParticipant);
            }

            Console.ReadKey();
        }
    }
}
 