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
        private static Dictionary<string, Tag> tags = new Dictionary<string, Tag>();

        private static readonly object locker = new object();

        public DatabaseManager()
        {
            TagProcessing.LoadTags(tags);
        }

        public bool ChangeOutputValue(double output, string tagName)
        {
            if (output < 0) return false;
            Tag tag;
            lock (locker)
            {
                try
                {
                    tag = tags[tagName];
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                if (tag is DigitalOutput && !(tag is AnalogOutput))
                {
                    if (output != 0 && output != 1) return false;
                    DigitalOutput digitalOutputTag = (DigitalOutput)tag;
                    digitalOutputTag.OutputValue = output;
                }
                else
                {
                    AnalogOutput analogOutputTag = (AnalogOutput)tag;
                    analogOutputTag.OutputValue = output;
                }
                TagProcessing.SaveTags(tags);
                return true;
            }
        }

        public double GetOutputValue(string tagName)
        {
            Tag tag;
            lock (locker)
            {
                try
                {
                    tag = tags[tagName];
                }
                catch (KeyNotFoundException)
                {
                    return -1000;
                }
                if (tag is DigitalOutput)
                {
                    DigitalOutput digitalOutputTag = (DigitalOutput)tag;
                    return digitalOutputTag.OutputValue;
                }
                return -1000;
            }
        }

        public bool ChangeScanState(string tagName)
        {
            Tag tag;
            lock (locker)
            {
                try
                {
                    tag = tags[tagName];
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                if (tag is DigitalInput)
                {
                    DigitalInput digitalInputTag = (DigitalInput)tag;
                    digitalInputTag.OnOffScan = !digitalInputTag.OnOffScan;
                    TagProcessing.SaveTags(tags);
                    return true;
                }
                return false;
            }
        }

        public bool AddDigitalInputTag(string tagName, string description, string driver, 
            string ioAddress, int scanTime, bool onOffScan)
        {
            Tag tag = new DigitalInput(tagName, description, ioAddress, driver, scanTime, onOffScan);
            lock (locker)
            {
                tags.Add(tagName, tag);
                TagProcessing.SaveTags(tags);
                return true;
            }
        }

        public bool AddDigitalOutputTag(string tagName, string description,
            string ioAddress, double initialValue)
        {
            Tag tag = new DigitalOutput(tagName, description, ioAddress, initialValue);
            lock (locker)
            {
                tags.Add(tagName, tag);
                TagProcessing.SaveTags(tags);
                return true;
            }
        }

        public bool AddAnalogInputTag(string tagName, string description, string driver, 
            string ioAddress, int scanTime, bool onOffScan, double lowLimit, double highLimit, string units)
        {
            Tag tag = new AnalogInput(tagName, description, ioAddress, driver, scanTime, 
                onOffScan, lowLimit, highLimit, units);
            lock (locker)
            {
                tags.Add(tagName, tag);
                TagProcessing.SaveTags(tags);
                return true;
            }
        }

        public bool AddAnalogOutputTag(string tagName, string description, 
            string ioAddress, double initialValue, double lowLimit, double highLimit, string units)
        {
            Tag tag = new AnalogOutput(tagName, description, ioAddress, initialValue, 
                lowLimit, highLimit, units);
            lock (locker)
            {
                tags.Add(tagName, tag);
                TagProcessing.SaveTags(tags);
                return true;
            }
        }

        public bool RemoveTag(string tagName)
        {
            lock (locker)
            {
                if (tags.Remove(tagName))
                {
                    TagProcessing.SaveTags(tags);
                    return true;
                }
                return false;
            }
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
                        lock (locker)
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
            lock (locker)
            {
                return authenticatedUsers.Remove(token);
            }
        }

        public bool Registration(string username, string password)
        {
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
    }
}
