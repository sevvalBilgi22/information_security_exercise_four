using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace App3_Verifierr
{
    class Program
    {
        static void Main()
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5003);
            server.Start();
            Console.WriteLine("App 3 (Verifier): Listening (Port 5003)...");

            using (TcpClient client = server.AcceptTcpClient())
            using (NetworkStream stream = client.GetStream())
            using (BinaryReader reader = new BinaryReader(stream)) // JSON yerine BinaryReader geldi
            {
                // Verileri gönderiliş sırasıyla okuyoruz
                string message = reader.ReadString();
                string signatureBase64 = reader.ReadString();
                string publicKeyXml = reader.ReadString();

                Console.WriteLine("\n--- App 3: Data Recieved ---");
                Console.WriteLine($"Mesaj: {message}");

                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] signatureBytes = Convert.FromBase64String(signatureBase64);

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(publicKeyXml);

                    Console.WriteLine("\nVerification is starting...");
                    bool isValid = rsa.VerifyData(messageBytes, CryptoConfig.MapNameToOID("SHA256"), signatureBytes);

                    if (isValid)
                        Console.WriteLine("Result: Signature is valid. Message is safe.");
                    else
                        Console.WriteLine("Result: Sİgnature is invalid! Message or signature is changed.");
                }
            }
            Console.ReadLine();
        }
    }
}