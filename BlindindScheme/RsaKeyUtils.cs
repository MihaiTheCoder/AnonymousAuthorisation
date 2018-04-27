using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BlindindScheme
{
    public class RsaKeyUtils
    {
        public static string GetSerializedPublicKey(RsaKeyParameters publicKey)
        {
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            string serializedPublic = Convert.ToBase64String(serializedPublicBytes);
            return serializedPublic;
        }

        public static string GetSerializedPrivateKey(RsaKeyParameters privateKey)
        {
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();
            string serializedPrivate = Convert.ToBase64String(serializedPrivateBytes);

            return serializedPrivate;
        }

        public static byte[] GetSerializedPrivateKey(AsymmetricKeyParameter keyPair)
        {
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();
            return serializedPrivateBytes;
        }

        public static byte[] GetSerializedPublicKey(AsymmetricKeyParameter keyPair)
        {
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            return serializedPublicBytes;
        }

        public static RsaKeyParameters GetDeserializedKPublicKey(string serializedPublicKey)
        {
             RsaKeyParameters publicKey = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(serializedPublicKey));
            return publicKey;
        }

        public static RsaKeyParameters GetDeserializedPrivateKey(string serializedPrivateKey)
        {
            RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(serializedPrivateKey));
            return privateKey;

        }

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
    }
}
