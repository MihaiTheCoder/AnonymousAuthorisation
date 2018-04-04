using BlindChatCore.Certificate;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            FixedSizedQueue<string> lastLines = new FixedSizedQueue<string>(200);
            foreach (var line in File.ReadLines("E:/Backups/prod_malocore.sql"))
            {
                lastLines.Enqueue(line);
            }
            CertificateGenerator generator = new CertificateGenerator();
            generator.GenerateCertificate("C=DE,O=Organiztion", TimeSpan.FromDays(1), "cert.pfx", "Test.123");
            generator.LoadCertificate("cert.pfx", "Test.123");
        }
    }

    public class FixedSizedQueue<T>
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
    }
}
