using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using RTU.ServiceReference;

namespace RTU
{
    class Program
    {
        static RealTimeUnitClient realTimeUnitClient = new RealTimeUnitClient();
        private static CspParameters csp;
        private static RSACryptoServiceProvider rsa;
        const string EXPORT_FOLDER = @"C:\public_key\";
        const string PUBLIC_KEY_FILE = @"rsaPublicKey.txt";

        static void Main(string[] args)
        {
            string id, lowLimit, highLimit, address;
            while (true)
            {
                Console.WriteLine("----- REAL TIME UNIT SCREEN -----");
                Console.Write("Unit id: ");
                id = Console.ReadLine();
                Console.Write("Low limit: ");
                lowLimit = Console.ReadLine();
                Console.Write("High limit: ");
                highLimit = Console.ReadLine();
                Console.Write("Realtime driver address: ");
                address = Console.ReadLine();
                string response = CheckParameters(lowLimit, highLimit);
                if (response != "")
                {
                    Console.WriteLine(response);
                    continue;
                }
                break;
            }
            CreateAsmKeys();
            ExportPublicKey();

            string message = id + address + lowLimit + highLimit;
            var signature = SignMessage(message);

            // PROVERAAA
            if (!realTimeUnitClient.Init(id, address, signature, message))
            {
                Console.WriteLine("SOMETHING WENT WRONG!");
                Console.ReadKey();
                Environment.Exit(1);
            }

            Random random = new Random();
            Console.WriteLine("Starting...");
            while (true)
            {
                int value = random.Next(Convert.ToInt32(lowLimit), Convert.ToInt32(highLimit));
                realTimeUnitClient.UpdateValue(address, value);
                Thread.Sleep(5000);
            }
        }

        private static string CheckParameters(string lowLimit, string highLimit)
        {
            try
            {
                int lowLimitValue = Convert.ToInt32(lowLimit);
                if (lowLimitValue < 0)
                {
                    return "LOW LIMIT MUST BE A POSITIVE INTEGER";
                }
            }
            catch (FormatException)
            {
                return "LOW LIMIT MUST BE A NUMBER";
            }
            try
            {
                int highLimitValue = Convert.ToInt32(highLimit);
                int lowLimitValue = Convert.ToInt32(lowLimit);
                if (highLimitValue < 0)
                {
                    return "HIGH LIMIT MUST BE A POSITIVE INTEGER";
                }
                else if (highLimitValue < lowLimitValue)
                {
                    return "HIGH LIMIT MUST BE GREATER THAN LOW LIMIT";
                }
            }
            catch (FormatException)
            {
                return "HIGH LIMIT MUST BE A NUMBER";
            }
            return "";
        }

        public static void CreateAsmKeys()
        {
            csp = new CspParameters();
            rsa = new RSACryptoServiceProvider(csp);
        }
        private static byte[] SignMessage(string message)
        {
            using (SHA256 sha = SHA256Managed.Create())
            {
                var hashValue = sha.ComputeHash(Encoding.UTF8.GetBytes(message));
                var formatter = new RSAPKCS1SignatureFormatter(rsa);
                formatter.SetHashAlgorithm("SHA256");
                return formatter.CreateSignature(hashValue);
            }
        }
        private static void ExportPublicKey()
        {
            if (!(Directory.Exists(EXPORT_FOLDER)))
                Directory.CreateDirectory(EXPORT_FOLDER);
            string path = Path.Combine(EXPORT_FOLDER, PUBLIC_KEY_FILE);
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(rsa.ToXmlString(false));
            }
        }
    }
}
