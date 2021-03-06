using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace SNUSProjekat
{
    public class DatabaseManager : IAuthentication, IDatabaseManager
    {
        private static Dictionary<string, User> authenticatedUsers = 
            new Dictionary<string, User>();

        private static readonly object userLocker = new object();

        public bool ChangeOutputValue(double output, string tagName)
        {
            return TagProcessing.ChangeOutputValue(output, tagName);
        }

        public double GetOutputValue(string tagName)
        {
            return TagProcessing.GetOutputValue(tagName);
        }

        public bool ChangeScanState(string tagName)
        {
            return TagProcessing.ChangeScanState(tagName);
        }

        public bool AddDigitalInputTag(string tagName, string description, string driver, 
            string ioAddress, int scanTime, bool onOffScan)
        {
            return TagProcessing.AddDigitalInputTag(tagName, description, ioAddress, 
                driver, scanTime, onOffScan);
        }

        public bool AddDigitalOutputTag(string tagName, string description,
            string ioAddress, double initialValue)
        {
            return TagProcessing.AddDigitalOutputTag(tagName, description, ioAddress, initialValue);
        }

        public bool AddAnalogInputTag(string tagName, string description, string driver, 
            string ioAddress, int scanTime, bool onOffScan, double lowLimit, double highLimit, 
            string units)
        {
            return TagProcessing.AddAnalogInputTag(tagName, description, ioAddress, driver, scanTime,
                onOffScan, lowLimit, highLimit, units);
        }

        public bool AddAnalogOutputTag(string tagName, string description, 
            string ioAddress, double initialValue, double lowLimit, double highLimit, string units)
        {
            return TagProcessing.AddAnalogOutputTag(tagName, description, ioAddress, initialValue,
                lowLimit, highLimit, units);
        }

        public bool RemoveTag(string tagName)
        {
            return TagProcessing.RemoveTag(tagName);
        }

        public string Login(string username, string password)
        {
            using (var db = new UsersContext())
            {
                foreach (var user in db.Users)
                {
                    if (username == user.Username &&
                        ValidateEncryptedData(password, user.EncryptedPassword))
                    {
                        string token = GenerateToken(username);
                        lock (userLocker)
                        {
                            authenticatedUsers.Add(token, user);
                        }
                        return token;
                    }
                }
            }
            return "Login failed";
        }

        public bool Logout(string token)
        {
            lock (userLocker)
            {
                return authenticatedUsers.Remove(token);
            }
        }

        public bool Registration(string username, string password)
        {
            if (username == "" || password == "") return false;
            string encryptedPassword = EncryptData(password);
            User user = new User(username, encryptedPassword);
            using (var db = new UsersContext())
            {
                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        private static string EncryptData(string valueToEncrypt)
        {
            string GenerateSalt()
            {
                RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
                byte[] salt = new byte[32];
                crypto.GetBytes(salt);
                return Convert.ToBase64String(salt);
            }
            string EncryptValue(string strValue)
            {
                string saltValue = GenerateSalt();
                byte[] saltedPassword = Encoding.UTF8.GetBytes(saltValue + strValue);
                using (SHA256Managed sha = new SHA256Managed())
                {
                    byte[] hash = sha.ComputeHash(saltedPassword);
                    return $"{Convert.ToBase64String(hash)}:{saltValue}";
                }
            }
            return EncryptValue(valueToEncrypt);
        }

        private static bool ValidateEncryptedData(string valueToValidate, 
            string valueFromDatabase)
        {
            string[] arrValues = valueFromDatabase.Split(':');
            string encryptedDbValue = arrValues[0];
            string salt = arrValues[1];
            byte[] saltedValue = Encoding.UTF8.GetBytes(salt + valueToValidate);
            using (var sha = new SHA256Managed())
            {
                byte[] hash = sha.ComputeHash(saltedValue);
                string enteredValueToValidate = Convert.ToBase64String(hash);
                return encryptedDbValue.Equals(enteredValueToValidate);
            }
        }

        private string GenerateToken(string username)
        {
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] randVal = new byte[32];
            crypto.GetBytes(randVal);
            string randStr = Convert.ToBase64String(randVal);
            return username + randStr;
        }

        public void LoadScadaConfig()
        {
            TagProcessing.LoadTags();
        }

        public bool AddAlarm(string tagName, string type, int priority, double limit)
        {
            return TagProcessing.AddAlarm(tagName, type, priority, limit);
        }

        public bool RemoveAlarm(string tagName, string type, int priority, double limit)
        {
            return TagProcessing.RemoveAlarm(tagName, type, priority, limit);
        }
    }
}
