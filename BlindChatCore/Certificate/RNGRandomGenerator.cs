using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BlindChatCore.Certificate
{
    public class RNGRandomGenerator : IRandomGenerator, ISimpleRandomGenerator
    {
        private readonly RNGCryptoServiceProvider rndProv;

        public RNGRandomGenerator()
        {
            rndProv = new RNGCryptoServiceProvider();
        }
        public void AddSeedMaterial(byte[] seed)
        {
            // We don't care about the seed
        }

        public void AddSeedMaterial(long seed)
        {
            // We don't care about the seed
        }

        public void NextBytes(byte[] bytes)
        {
            rndProv.GetBytes(bytes);
        }

        public void NextBytes(byte[] bytes, int start, int len)
        {
            if (start < 0)
                throw new ArgumentException("Start offset cannot be negative", "start");
            if (bytes.Length < (start + len))
                throw new ArgumentException("Byte array too small for requested offset and length");

            if (bytes.Length == len && start == 0)
            {
                NextBytes(bytes);
            }
            else
            {
                byte[] tmpBuf = new byte[len];
                rndProv.GetBytes(tmpBuf);
                Array.Copy(tmpBuf, 0, bytes, start, len);
            }
        }

        public int Next(int min, int max)
        {
            if (min > max) throw new ArgumentOutOfRangeException(nameof(min));
            if (min == max) return min;

            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[4];
                rng.GetBytes(data);

                int generatedValue = Math.Abs(BitConverter.ToInt32(data, startIndex: 0));

                int diff = max - min;
                int mod = generatedValue % diff;
                int normalizedNumber = min + mod;

                return normalizedNumber;
            }
        }
    }
}
