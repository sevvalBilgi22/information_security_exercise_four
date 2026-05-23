using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace App2_Proxy
{
    class Program
    {
        static void Main()
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5002);
            server.Start();
            Console.WriteLine("App 2 (Proxy): Listening (Port 5002)...");

            using (TcpClient incomingClient = server.AcceptTcpClient())
            using (NetworkStream incomingStream = incomingClient.GetStream())
            using (BinaryReader reader = new BinaryReader(incomingStream))
            {
                // App 1'den oku
                string message = reader.ReadString();
                string signatureBase64 = reader.ReadString();
                string publicKeyXml = reader.ReadString();

                Console.WriteLine("\n--- App 2: Data Recieved from App 1 ---");
                Console.WriteLine($"Orginal Message: {message}");
                Console.WriteLine($"Orginal Signature (First 20 character): {signatureBase64.Substring(0, 20)}...");

                Console.Write("\n[!] Do you want to change the signature manually? (Y/N): ");
                string tamper = Console.ReadLine();

                if (tamper.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    signatureBase64 = "AA" + signatureBase64.Substring(2);
                    Console.WriteLine("Warning: Signature has changed!");
                }
                else
                {
                    Console.WriteLine("Signature hasnt changed.");
                }

                // App 3'e aktar
                using (TcpClient outgoingClient = new TcpClient("127.0.0.1", 5003))
                using (NetworkStream outgoingStream = outgoingClient.GetStream())
                using (BinaryWriter writer = new BinaryWriter(outgoingStream))
                {
                    writer.Write(message);
                    writer.Write(signatureBase64);
                    writer.Write(publicKeyXml);
                    Console.WriteLine("App 2: Data is delivered ro App 3.");
                }
            }
            Console.ReadLine();
        }
    }
}
