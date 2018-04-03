using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

namespace BlindChatCore.Certificate
{
    public class CertificateGenerator
    {
        const string signatureAlgorithm = "SHA256WithRSA";
        const int strength = 2048;

        public AsymmetricCipherKeyPair GenerateCertificate(string subjectName, TimeSpan validFor, string path, string password)
        {
            SecureRandom random = GetRandom();

            var certificateGenerator = new X509V3CertificateGenerator();

            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);
            certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);

            var subjectDN = new X509Name(subjectName);
            var issuerDN = subjectDN;
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            var notBefore = DateTime.UtcNow.Date;
            var notAfter = notBefore.Add(validFor);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            var keyGenerationParameters = new KeyGenerationParameters(random, strength);

            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            AsymmetricCipherKeyPair subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            var issuerKeyPair = subjectKeyPair;
            var certificate = certificateGenerator.Generate(issuerKeyPair.Private, random);

            var store = new Pkcs12Store();
            string friendlyName = certificate.SubjectDN.ToString();
            var certificateEntry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(friendlyName, certificateEntry);
            var stream = new MemoryStream();
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { certificateEntry });
            store.Save(stream, password.ToCharArray(), random);
            File.WriteAllBytes(path, stream.ToArray());
            return subjectKeyPair;
        }

        private static SecureRandom GetRandom()
        {
            var randomGenerator = new RNGRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            return random;
        }

        public Pkcs12Store LoadCertificate( string path, string password)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var certificate = new Pkcs12Store(stream, password.ToCharArray());
                return certificate;
            }
                
        }
    }
}
