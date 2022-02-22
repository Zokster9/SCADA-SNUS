using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace SNUSProjekat
{
    public class RealTimeUnitService : IRealTimeUnit
    {
        private CspParameters csp;
        private RSACryptoServiceProvider rsa;
        const string IMPORT_FOLDER = @"C:\public_key\";
        const string PUBLIC_KEY_FILE = @"rsaPublicKey.txt";
        static readonly object locker = new object();
        static List<string> RealTimeDriverIds = new List<string>();

        public RealTimeUnitService()
        {
            ImportPublicKey();
        }

        public bool Init(string id, string address, byte[] signature, string message)
        {
            if (RealTimeDriver.values.ContainsKey(address) || RealTimeDriverIds.Contains(id))
            {
                return false;
            }
            if (VerifySignedMessage(message, signature))
            {
                lock (locker)
                {
                    RealTimeDriver.values[address] = 0;
                }
                RealTimeDriverIds.Add(id);
                return true;
            }
            return false;
        }

        public void UpdateValue(string address, int value)
        {
            lock (locker)
            {
                RealTimeDriver.values[address] = value;
            }
        }

        public bool VerifySignedMessage(string message, byte[] signature)
        {
            using (var sha = SHA256Managed.Create())
            {
                var hashValue = sha.ComputeHash(Encoding.UTF8.GetBytes(message));

                var deformatter = new RSAPKCS1SignatureDeformatter(rsa);
                deformatter.SetHashAlgorithm("SHA256");

                return deformatter.VerifySignature(hashValue, signature);
            }
        }

        public void ImportPublicKey()
        {
            string path = Path.Combine(IMPORT_FOLDER, PUBLIC_KEY_FILE);
            FileInfo fi = new FileInfo(path);

            if (fi.Exists)
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    csp = new CspParameters();
                    rsa = new RSACryptoServiceProvider(csp);
                    string publicKeyText = reader.ReadToEnd();
                    rsa.FromXmlString(publicKeyText);
                }
            }
        }
    }
}
