using BlindChatCore.Certificate;
using System;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            CertificateGenerator generator = new CertificateGenerator();
            generator.GenerateCertificate("C=DE,O=Organiztion", TimeSpan.FromDays(1), "cert.pfx", "Test.123");
            generator.LoadCertificate("cert.pfx", "Test.123");
        }
    }
}
