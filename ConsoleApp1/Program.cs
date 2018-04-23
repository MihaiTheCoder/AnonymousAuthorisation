using BlindChatCore.Certificate;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using BlindChatCore.Model;
using BlindChat.Infrastructure.Repository;
using BlindChatCore.Server;
using BlindindScheme;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using BlindindScheme.SignatureRequester;
using Org.BouncyCastle.Crypto;
using BlindChatCore;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            /* FixedSizedQueue<string> lastLines = new FixedSizedQueue<string>(200);
             foreach (var line in File.ReadLines("E:/Backups/prod_malocore.sql"))
             {
                 lastLines.Enqueue(line);
             }*/
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
            Group group = new Group();
            group.Name = "Loazarii";
            group.OwnerEmail = "ioan.morar@qubiz.com";
            group.RsaPublicKey = (RsaKeyParameters)groupKeys.Public;
            groupCreator.RegisterGroup(group);

            Console.WriteLine("Create group: " + group.Name);

            //Send invites
            //TODO: new method to be implemented
            //Add participant to group
            List<string> emails = new List<string>();
            var participantEmail = "andreiu_fitzosul_01@gmai.com";
            emails.Add(participantEmail);

            var getGroup = context.Groups.FirstOrDefault(g => g.Name == group.Name);
            var getConfirmationCode = context.ConfirmationCodes.FirstOrDefault(c => c.GroupId == getGroup.Id);
            int confirmatioCode = getConfirmationCode.Code;
            server.AddParticipantsToGroup(emails, confirmatioCode);

            Console.WriteLine("Add participants to group");

            //Participant
            var invitationCode = server.groupDetailDictionary.Keys.Last();
            
            ClientParticipant clientParticipant = new ClientParticipant(server, groupRepository);
            var groupPublicKey = clientParticipant.GetGroupDetails(invitationCode);

            //Generate participant Certificate
            var participantKeys = generator.GenerateCertificate("C=RO,O=Qubiz", TimeSpan.FromDays(1), "certParticipant.pfx", "Test.123");

            //Serialize PublicKey and PrivateKey
            var privateSerializedKey = RsaKeyUtils.GetSerializedPrivateKey(participantKeys.Private);
            var publicSerializedKey = RsaKeyUtils.GetSerializedPublicKey(participantKeys.Public);

            //Concatenante serialized key
            var content = RsaKeyUtils.Combine(publicSerializedKey, privateSerializedKey);

            //Generate blind message
            ContentBlinder contentBlinder = new ContentBlinder((RsaKeyParameters)groupKeys.Public);
            var blindedContent = contentBlinder.GetBlindedContent(content);
            Console.WriteLine("MESSAGE TO BE SIGNED BY THE GROUP:");
            Console.WriteLine("");
            Console.WriteLine(Base64.ToBase64String(blindedContent));
            Console.WriteLine("");

            //TODO: Send for sign (email, invitationCode, blindMessage)

            //GroupCreator: recive blind message (inviteCode, blindMessage)

            //GroupCreator : removeInvitationCode() from dictionary
            //server.groupDetailDictionary.Remove(invitationCode);

            //GroupCreator: BlindSigner (GroupCreator.PrivateKey, BlindMessage)
            var signature = blindSigner.Sign(blindedContent);
            Console.WriteLine("THE GROUP'S SIGNATURE:");
            Console.WriteLine("");
            Console.WriteLine(Base64.ToBase64String(signature));
            Console.WriteLine("");

            //GroupCreator: SendToParticipant(email, blindMessage, blindSignature)

            //Participant ReceiveBlindSignature()

            //Unblind signedBlindMessage
            var unblindedSignature = contentBlinder.GetUnblindedSignature(signature);
            Console.WriteLine("UNBLINDED SIGNATURE:");
            Console.WriteLine("");
            Console.WriteLine(Base64.ToBase64String(unblindedSignature));
            Console.WriteLine("");

            //Send(ParticipantPublicKey, UnblindSignedMessage,Text = 'Am I right? Am you right.')

            //Verify 
            SignedEntity signedEntity = new SignedEntity(content, unblindedSignature);
            var valid = signatureVerifier.Verify(signedEntity, groupKeys.Public);
           
            if (valid)
            {
                Console.WriteLine("Well done!");
            }
            else
            {
                Console.WriteLine("Ce facut ma nene ma?");
            }

            //Deserialize PublicKey and PrivateKey
            //RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(serializedPrivate));
            //RsaKeyParameters publicKey = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(serializedPublic));                  
            Console.ReadKey();
        }


    }


   /* public class FixedSizedQueue<T>
    {
        public FixedSizedQueue(int limit)
        {
            Limit = limit;
        }
        Queue<T> q = new Queue<T>();

        public int Limit { get; set; }
        public void Enqueue(T obj)
        {
            q.Enqueue(obj);

            T overflow;
            while (q.Count > Limit && q.TryDequeue(out overflow)) ;

        }
    }*/
}
