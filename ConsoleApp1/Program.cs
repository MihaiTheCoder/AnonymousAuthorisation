using BlindChatCore.Certificate;
using System;
using System.Linq;
using System.Collections.Generic;
using BlindChatCore.Model;
using BlindChat.Infrastructure.Repository;
using BlindChatCore.Server;
using BlindindScheme;
using Org.BouncyCastle.Crypto.Parameters;
using BlindindScheme.SignatureRequester;
using BlindChatCore;
using Org.BouncyCastle.Utilities.Encoders;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            SignatureVerifier signatureVerifier = new SignatureVerifier();
            RNGRandomGenerator rngGenerator = new RNGRandomGenerator();
            EmailSender emailSender = new EmailSender();
            BlindChatDbContext context = new BlindChatDbContext();
            GroupRepository groupRepository = new GroupRepository(context);
            APIServer server = new APIServer(groupRepository, emailSender, rngGenerator, signatureVerifier);


            //GroupCreator
            /* 1. Generate certificate
             * 2. Create group
             * 3. Send invites
             */
            // Generate group Certificate
            CertificateGenerator generator = new CertificateGenerator();
            var groupKeys = generator.GenerateCertificate("C=DE,O=Organiztion", TimeSpan.FromDays(1), "cert.pfx", "Test.123");

            Console.WriteLine("Generate group certificate");

            // Create group
            BlindSigner blindSigner = new BlindSigner(groupKeys);
            GroupCreator groupCreator = new GroupCreator(server, blindSigner);
            Console.WriteLine("Create group");
            //Console.WriteLine("Enter group name:");
            //string groupName = Console.ReadLine();
            string groupName = "Loazarii";
            Group group = new Group();            
            group.Name = groupName;
            //Console.WriteLine("Enter owner email:");
            //string ownerEmail = Console.ReadLine();
            string ownerEmail = "jumanji@jungle.com";
            group.OwnerEmail = ownerEmail;
            group.RsaPublicKey = (RsaKeyParameters)groupKeys.Public;
            groupCreator.RegisterGroup(group);
            Console.WriteLine("");
            Console.WriteLine("You're group " + group.Name + " was registered!");


            //Send invites
            //TODO: new method to be implemented
            //Add participant to group
            List<string> emails = new List<string>();
            var participantEmail = "andreiu_fitzosul_01@gmai.com";
            emails.Add(participantEmail);
            
            var getGroup = context.Groups.FirstOrDefault(g => g.Name == group.Name);
            var getConfirmationCode = context.ConfirmationCodes.FirstOrDefault(c => c.GroupId == getGroup.Id);
            int confirmatioCode = getConfirmationCode.Code;
            groupCreator.AddParticipantsToGroup(emails, confirmatioCode);

            Console.WriteLine("Add participants to group");

            List<Participant> unconfirmedParticipants = server.GetParticipantsToConfirm();
            foreach (var participant in unconfirmedParticipants)
            {
                int invitationCode = participant.InvitationCode;
                Guid groupId = (Guid)participant.GroupId;
                string email = participant.Email;
                Group user_group = server.GetGroup(participant.InvitationCode);

                ClientParticipant clientParticipant = new ClientParticipant(server, groupRepository);
                var groupPublicKey = clientParticipant.GetGroupDetails(invitationCode);

                //Generate certificate
                var participantKeys = generator.GenerateCertificate("C=RO,O=Qubiz", TimeSpan.FromDays(1), "certParticipant.pfx", "Test.123");
                Console.WriteLine("Client certificate was generated");
                
                //Create GroupRegistration
                var groupRegistration = clientParticipant.GetGroupRegistration(invitationCode, (RsaKeyParameters)participantKeys.Public);             

                //Save blindedCertificate 
                clientParticipant.RegisterBlindCertificate(invitationCode, groupRegistration);
                Console.WriteLine("Blind certificate was saved");

                //Send for sign DONE
                groupCreator.SignMessages(getGroup);
                Console.WriteLine("Blind message was signed");
                
                //Add BlindParticipant               
                VerifiedParticipant verifiedParticipant = clientParticipant.CheckVerifiedEntity(user_group, email, groupRegistration);
                clientParticipant.AddClientCertificate(verifiedParticipant, user_group, email);
                clientParticipant.AddBlindParticipant(user_group.Id, verifiedParticipant);
                ParticipantMessage message = new ParticipantMessage();
                message.Message = "Andreiu, ce nevoie faci?";

                //Verify
                var blindparticipants = groupCreator.GetBlindParticipants(groupId);
                foreach (var blindparticipant in blindparticipants)
                {
                    clientParticipant.AddMessage(groupId, message, blindparticipant);
                }
                                
                Console.WriteLine("Participant verified");
                Console.WriteLine("Message sent");
            }
            Console.ReadKey();            
        }
    }
}
