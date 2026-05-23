using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace App1_Sender
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("App 1 (Sender):Creating RSA Keys...");

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                Console.Write("Type the message will sent: ");
                string message = Console.ReadLine();

                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                byte[] signatureBytes = rsa.SignData(messageBytes, CryptoConfig.MapNameToOID("SHA256"));
                string signatureBase64 = Convert.ToBase64String(signatureBytes);
                string publicKeyXml = rsa.ToXmlString(false);

                Console.WriteLine("\nMessage is signed.");

                using (TcpClient client = new TcpClient("127.0.0.1", 5002))
                using (NetworkStream stream = client.GetStream())
                using (BinaryWriter writer = new BinaryWriter(stream)) // JSON kütüphanesi yerine BinaryWriter
                {
                    // Sırayla yolla
                    writer.Write(message);
                    writer.Write(signatureBase64);
                    writer.Write(publicKeyXml);

                    Console.WriteLine("App 1: Message, signature and public key delivered to App 2.");
                }
            }
            Console.ReadLine();
        }
    }
}
