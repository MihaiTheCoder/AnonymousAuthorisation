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
using Org.BouncyCastle.Crypto;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Org.BouncyCastle.X509;
using System.IO;
using Org.BouncyCastle.Math;

namespace ConsoleApp1
{
    class Program
    {

        static public int DisplayMenu()
        {
            // Console menu
            Console.WriteLine("Welcome to our anonymous chat please select an option:");
            Console.WriteLine();
            Console.WriteLine("1. Create anonymous chat group");
            Console.WriteLine("2. Invite participants to your anonymous chat group");
            Console.WriteLine("3. Register a participant to anonymous chat group");
            Console.WriteLine("4. Sign participant certificate");
            Console.WriteLine("5. Save blind participant");
            Console.WriteLine("6. Send messages on group");
            Console.WriteLine("7. Exit");
            Console.WriteLine("Choose an option:");
            var result = Console.ReadLine();

            return Convert.ToInt32(result);
        }

        static public void CreateGroup(APIServer server, CertificateGenerator generator)
        {
            // Generate group Certificate
            var groupKeys = generator.GenerateCertificate("C=DE,O=Organiztion", TimeSpan.FromDays(1), "cert.pfx", "Test.123");
            Console.WriteLine("Group certificate was generated");

            BlindSigner blindSigner = new BlindSigner(groupKeys);
            GroupCreator groupCreator = new GroupCreator(server, blindSigner);

            Console.WriteLine("Create group");
            Console.WriteLine("Enter group name:");
            string groupName = Console.ReadLine();
            //string groupName = "Loazarii";
            Group group = new Group();
            group.Name = groupName;
            Console.WriteLine("Enter owner email:");
            string ownerEmail = Console.ReadLine();
            //string ownerEmail = "jumanji@jungle.com";
            group.OwnerEmail = ownerEmail;
            group.RsaPublicKey = (RsaKeyParameters)groupKeys.Public;
            groupCreator.RegisterGroup(group);
            Console.WriteLine("");

            //Write keys to file
            File.WriteAllText(group.Name + "PublicKey.txt", RsaKeyUtils.GetSerializedPublicKey((RsaKeyParameters)groupKeys.Public));
            File.WriteAllText(group.Name + "PrivateKey.txt", RsaKeyUtils.GetSerializedPrivateKey((RsaKeyParameters)groupKeys.Private));

            Console.WriteLine("You're group " + group.Name + " was registered!");
        }

        static public void InviteParticipants(BlindChatDbContext context, APIServer server)
        {
            Console.WriteLine("Enter the group for which you want to invite participants:");
            string groupName = Console.ReadLine();
            string pubKeyFile = groupName + "PublicKey.txt";
            string privKeyFile = groupName + "PrivateKey.txt";

            if (pubKeyFile != null && privKeyFile != null)
            {
                var groupCreator = GetGroupCreator(server, pubKeyFile, privKeyFile);
                List<string> emails = new List<string>();
                Console.WriteLine("Enter participant email address:");
                var participantEmail = Console.ReadLine();
                //var participantEmail = "andreiu_fitzosul_01@gmai.com";
                emails.Add(participantEmail);
                Console.WriteLine();

                var getGroup = context.Groups.FirstOrDefault(g => g.Name == groupName);
                var getConfirmationCode = context.ConfirmationCodes.FirstOrDefault(c => c.GroupId == getGroup.Id);
                int confirmatioCode = getConfirmationCode.Code;
                groupCreator.AddParticipantsToGroup(emails, confirmatioCode);

                Console.WriteLine("Participant was added to the group");
            }
            else
            {
                Console.WriteLine("Group creator Keys were not saved to file, please go to step 1");
            }
        }

        static public AsymmetricCipherKeyPair ImportCertificate(string pubKeyFile, string privKeyFile)
        {
            var pubKey = (AsymmetricKeyParameter)RsaKeyUtils.GetDeserializedKPublicKey(File.ReadAllText(pubKeyFile));
            var privKey = (AsymmetricKeyParameter)RsaKeyUtils.GetDeserializedPrivateKey(File.ReadAllText(privKeyFile));
            AsymmetricCipherKeyPair keys = new AsymmetricCipherKeyPair(pubKey, privKey);

            return keys;
        }

        static public GroupCreator GetGroupCreator(APIServer server, string pubKeyFile, string privKeyFile)
        {
            var keys = ImportCertificate(pubKeyFile, privKeyFile);
            BlindSigner blindSigner = new BlindSigner(keys);
            GroupCreator groupCreator = new GroupCreator(server, blindSigner);

            return groupCreator;
        }

        static public void RegisterParticipant(APIServer server, GroupRepository groupRepository, CertificateGenerator generator)
        {
            Console.WriteLine("Enter the group for which you want to register participants:");
            string groupName = Console.ReadLine();
            string pubKeyFile = groupName + "PublicKey.txt";
            string privKeyFile = groupName + "PrivateKey.txt";

            if (pubKeyFile != null && privKeyFile != null)
            {
                var groupCreator = GetGroupCreator(server, pubKeyFile, privKeyFile);
                Console.WriteLine();
                Console.WriteLine("Enter participant email to be confirmed:");
                var participantEmail = Console.ReadLine();
                var participant = groupCreator.GetParticipantToConfirm(groupName, participantEmail);
                int invitationCode = participant.InvitationCode;
                Guid groupId = (Guid)participant.GroupId;
                string email = participant.Email;
                Group user_group = groupCreator.GetGroup(participant.InvitationCode);

                ClientParticipant clientParticipant = new ClientParticipant(server, groupRepository);
                var groupPublicKey = clientParticipant.GetGroupDetails(invitationCode);

                //Generate certificate
                var participantKeys = generator.GenerateCertificate("C=RO,O=Qubiz", TimeSpan.FromDays(1), "certParticipant.pfx", "Test.123");
                Console.WriteLine("Client certificate was generated");

                //Write keys to file
                File.WriteAllText(participantEmail.Substring(0, participantEmail.IndexOf("@")) + "PublicKey.txt", RsaKeyUtils.GetSerializedPublicKey((RsaKeyParameters)participantKeys.Public));
                File.WriteAllText(participantEmail.Substring(0, participantEmail.IndexOf("@")) + "PrivateKey.txt", RsaKeyUtils.GetSerializedPrivateKey((RsaKeyParameters)participantKeys.Private));
                Console.WriteLine("Participant keys were saved to file");

                //Create GroupRegistration
                var groupRegistration = clientParticipant.GetGroupRegistration(invitationCode, (RsaKeyParameters)participantKeys.Public);
                Console.WriteLine("Blind factor was saved");

                //Save blindedCertificate 
                clientParticipant.RegisterBlindCertificate(invitationCode, groupRegistration);
                Console.WriteLine("Blind certificate was saved");
            }
            else
            {
                Console.WriteLine("Group creator Keys were not saved to file, please go to step 1");
            }          
        }

        static public void SignCertificate(APIServer server)
        {
            Console.WriteLine("Enter the group for which you want to register participants:");
            string groupName = Console.ReadLine();
            string pubKeyFile = groupName + "PublicKey.txt";
            string privKeyFile = groupName + "PrivateKey.txt";

            var groupCreator = GetGroupCreator(server, pubKeyFile, privKeyFile);
            var group = server.GetGroupByName(groupName);
            groupCreator.SignMessages(group);
            Console.WriteLine("Blind message was signed");
        }

        static public void SaveBlindParticipant(APIServer server, GroupRepository groupRepository)
        {
            Console.WriteLine("Enter the group for which you want to register participants:");
            var groupName = Console.ReadLine();
            Console.WriteLine("Enter participant email address that you want to be saved as blind participant:");
            var participantEmail = Console.ReadLine();
            var group = server.GetGroupByName(groupName);

            var factor = File.ReadAllText("BlindFactor.txt");
            BigInteger blindFactor = new BigInteger(factor);

            var fileToRead = (participantEmail.Substring(0, participantEmail.IndexOf("@")) + "PublicKey.txt").ToString();
            RsaKeyParameters participantPublicKey = (RsaKeyParameters)RsaKeyUtils.GetDeserializedKPublicKey(File.ReadAllText(fileToRead));
            GroupRegistration groupRegistration = new GroupRegistration(group, new ContentBlinder(group.RsaPublicKey, blindFactor), participantPublicKey);

            ClientParticipant clientParticipant = new ClientParticipant(server, groupRepository);
            VerifiedParticipant verifiedParticipant = clientParticipant.CheckVerifiedEntity(group, participantEmail, groupRegistration);
            clientParticipant.AddClientCertificate(verifiedParticipant, group, participantEmail);
            Console.WriteLine("Enter nickname:");
            var nickname = Console.ReadLine();
            clientParticipant.AddBlindParticipant(group.Id, verifiedParticipant, nickname);

            Console.WriteLine();
            Console.WriteLine("Participant was saved as a blind participant to the group");
        }

        public static void SendMessageOnGroup(APIServer server, GroupRepository groupRepository)
        {
            Console.WriteLine("Enter the group on which you want to send messages:");
            var groupName = Console.ReadLine();
            string pubKeyFile = groupName + "PublicKey.txt";
            string privKeyFile = groupName + "PrivateKey.txt";
            Group group = server.GetGroupByName(groupName);

            if (pubKeyFile != null && privKeyFile != null)
            {
                var groupCreator = GetGroupCreator(server, pubKeyFile, privKeyFile);
                ParticipantMessage message = new ParticipantMessage();
                Console.WriteLine("Enter the message you want to send:");
                message.Message = Console.ReadLine();
                Console.WriteLine("Enter your nickname:");
                var nickname = Console.ReadLine();
                
                ClientParticipant clientParticipant = new ClientParticipant(server, groupRepository);
                var blindparticipant = groupCreator.GetBlindParticipantByNickname(group.Id, nickname);
                VerifiedParticipant verifiedParticipant = new VerifiedParticipant
                {
                    PublicKey = blindparticipant.PublicKey,
                    Signature = blindparticipant.Signature
                };
                message.BlindParticipant = blindparticipant.Id.ToString();

                clientParticipant.AddMessage(group.Id, message, verifiedParticipant);
                Console.WriteLine("Participant verified");
                Console.WriteLine("Message sent");
            }
            else
            {
                Console.WriteLine("Group creator Keys were not saved to file, please go to step 1");
            }
        }

        static void Main(string[] args)
        {
            SignatureVerifier signatureVerifier = new SignatureVerifier();
            RNGRandomGenerator rngGenerator = new RNGRandomGenerator();
            EmailSender emailSender = new EmailSender();
            BlindChatDbContext context = new BlindChatDbContext();
            GroupRepository groupRepository = new GroupRepository(context);
            APIServer server = new APIServer(groupRepository, emailSender, rngGenerator, signatureVerifier);
          
            CertificateGenerator generator = new CertificateGenerator();


            int userInput = 0;
            do
            {
                userInput = DisplayMenu();
                switch (userInput.ToString())
                {
                    case "1":
                        CreateGroup(server, generator);                       
                        break;
                    case "2":
                        InviteParticipants(context, server);                    
                        break;
                    case "3":
                        RegisterParticipant(server, groupRepository, generator);                        
                        break;
                    case "4":                                           
                        SignCertificate(server);
                        break;
                    case "5":                        
                        SaveBlindParticipant(server, groupRepository);
                        break;
                    case "6":
                        SendMessageOnGroup(server, groupRepository);
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
            } while (userInput != 7);           
        }        
    }
}
