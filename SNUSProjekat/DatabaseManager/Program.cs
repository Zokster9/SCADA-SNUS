using DatabaseManager.ServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthenticationClient authenticationClient = 
                new AuthenticationClient();
            DatabaseManagerClient databaseManagerClient = 
                new DatabaseManagerClient();
            string token = "";
            databaseManagerClient.LoadScadaConfig();
            while (true)
            {
                Console.WriteLine("----- DATABASE MANAGER -----");
                Console.WriteLine("CHOOSE AN OPTION:");
                Console.WriteLine("\t1. Login");
                Console.WriteLine("\t2. Add tag");
                Console.WriteLine("\t3. Remove tag");
                Console.WriteLine("\t4. Add alarm");
                Console.WriteLine("\t5. Remove alarm");
                Console.WriteLine("\t6. Turn scan on/off");
                Console.WriteLine("\t7. Change output value");
                Console.WriteLine("\t8. Get output value");
                Console.WriteLine("\t9. Register user");
                Console.WriteLine("\t10. Logout");
                Console.WriteLine("\t11. Exit");
                Console.Write("ENTER AN OPTION: ");
                string option = Console.ReadLine();
                try
                {
                    int optionValue = Convert.ToInt32(option);

                    switch (optionValue)
                    {
                        case 1:
                            token = LoginScreen(authenticationClient);
                            break;
                        case 2:
                            AddTagScreen(databaseManagerClient);
                            break;
                        case 3:
                            RemoveTagScreen(databaseManagerClient);
                            break;
                        case 4:
                            AddAlarm(databaseManagerClient);
                            break;
                        case 5:
                            RemoveAlarm(databaseManagerClient);
                            break;
                        case 6:
                            TurnScanOnOffScreen(databaseManagerClient);
                            break;
                        case 7:
                            ChangeOutputValueScreen(databaseManagerClient);
                            break;
                        case 8:
                            GetOutputValueScreen(databaseManagerClient);
                            break;
                        case 9:
                            RegisterScreen(authenticationClient);
                            break;
                        case 10:
                            Logout(token, authenticationClient);
                            break;
                        case 11:
                            Environment.Exit(0);
                            break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("WRONG INPUT");
                }
            }
        }

        private static void RemoveAlarm(DatabaseManagerClient databaseManagerClient)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("----- REMOVE ALARM SCREEN -----");
            Console.Write("Tag name: ");
            string tagName = Console.ReadLine();
            Console.Write("Type: ");
            string type = Console.ReadLine();
            Console.Write("Priority: ");
            string priority = Console.ReadLine();
            Console.Write("Limit: ");
            string limit = Console.ReadLine();
            string response = CheckAlarmParameters(type, priority, limit);
            if (response == "")
            {
                if (databaseManagerClient.RemoveAlarm(tagName, type.ToLower(), Convert.ToInt32(priority),
                    Convert.ToDouble(limit)))
                {
                    Console.WriteLine("SUCCESSFULLY REMOVED ALARM");
                }
                else
                {
                    Console.WriteLine("SOMETHING WENT WRONG!");
                }
            }
            else
            {
                Console.WriteLine(response);
            }
        }

        private static void AddAlarm(DatabaseManagerClient databaseManagerClient)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("----- ADD ALARM SCREEN -----");
            Console.Write("Tag name: ");
            string tagName = Console.ReadLine();
            Console.Write("Type: ");
            string type = Console.ReadLine();
            Console.Write("Priority: ");
            string priority = Console.ReadLine();
            Console.Write("Limit: ");
            string limit = Console.ReadLine();
            string response = CheckAlarmParameters(type, priority, limit);
            if (response == "")
            {
                if (databaseManagerClient.AddAlarm(tagName, type.ToLower(), Convert.ToInt32(priority), 
                    Convert.ToDouble(limit)))
                {
                    Console.WriteLine("SUCCESSFULLY ADDED ALARM");
                }
                else
                {
                    Console.WriteLine("SOMETHING WENT WRONG!");
                }
            }
            else
            {
                Console.WriteLine(response);
            }
        }

        private static string CheckAlarmParameters(string type, string priority, string limit)
        {
            if (type != "low" && type != "high")
            {
                return "TYPE MUST BE LOW OR HIGH!";
            }
            try
            {
                int priorityValue = Convert.ToInt32(priority);
                if (priorityValue != 1 && priorityValue != 2 && priorityValue != 3)
                {
                    return "PRIORITY MUST BE 1, 2, OR 3";
                }
            }
            catch (FormatException)
            {
                return "PRIORITY MUST BE A NUMBER";
            }
            try
            {
                double limitValue = Convert.ToDouble(limit);
            }
            catch (FormatException)
            {
                return "LIMIT MUST BE A NUMBER";
            }
            return "";
        }

        private static string LoginScreen(AuthenticationClient authenticationClient)
        {
            while (true)
            {
                Console.WriteLine("-------------------");
                Console.WriteLine("----- LOGIN SCREEN -----");
                Console.Write("Username: ");
                string username = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                string provera = authenticationClient.Login(username, password);
                if (provera != "Login failed")
                {
                    Console.WriteLine("SUCCESSFUL LOGIN");
                    return provera;
                }
                else
                {
                    Console.WriteLine(provera + ". Try again.");
                }
            }
        }

        private static void AddTagScreen(DatabaseManagerClient databaseManagerClient)
        {
            bool exit = false;
            while (true)
            {
                Console.WriteLine("-------------------");
                Console.WriteLine("----- ADD TAG SCREEN -----");
                Console.WriteLine("\t1. Add digital input tag");
                Console.WriteLine("\t2. Add digital output tag");
                Console.WriteLine("\t3. Add analog input tag");
                Console.WriteLine("\t4. Add analog output tag");
                Console.WriteLine("\t5. Exit");
                Console.Write("ENTER AN OPTION: ");
                string option = Console.ReadLine();
                try
                {
                    int optionValue = Convert.ToInt32(option);

                    switch (optionValue)
                    {
                        case 1:
                            AddDigitalInputTag(databaseManagerClient);
                            break;
                        case 2:
                            AddDigitalOutputTag(databaseManagerClient);
                            break;
                        case 3:
                            AddAnalogInputTag(databaseManagerClient);
                            break;
                        case 4:
                            AddAnalogOutputTag(databaseManagerClient);
                            break;
                        case 5:
                            exit = true;
                            break;
                    }
                    if (exit) break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("WRONG INPUT");
                }
            }
        }

        private static void RemoveTagScreen(DatabaseManagerClient databaseManagerClient)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("----- REMOVE TAG SCREEN -----");
            Console.Write("Enter tag name: ");
            string tagName = Console.ReadLine();
            if (databaseManagerClient.RemoveTag(tagName))
            {
                Console.WriteLine("TAG SUCCESSFULLY DELETED");
            }
            else
            {
                Console.WriteLine("TAG WAS NOT DELETED");
            }
        }

        private static void TurnScanOnOffScreen(DatabaseManagerClient databaseManagerClient)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("----- TURN SCAN ON/OFF SCREEN -----");
            Console.Write("Enter tag name: ");
            string tagName = Console.ReadLine();
            if (databaseManagerClient.ChangeScanState(tagName))
            {
                Console.WriteLine("SCAN SUCCESSFULLY TURNED ON/OFF");
            }
            else
            {
                Console.WriteLine("SCAN COULD NOT BE TURNED ON/OFF");
            }
        }

        private static void ChangeOutputValueScreen(DatabaseManagerClient databaseManagerClient)
        {
            while (true)
            {
                Console.WriteLine("-------------------");
                Console.WriteLine("----- CHANGE OUTPUT VALUE SCREEN -----");
                Console.Write("Enter tag name: ");
                string tagName = Console.ReadLine();
                Console.Write("Enter new value: ");
                string newValue = Console.ReadLine();
                double value = 0;
                try
                {
                    value = Convert.ToDouble(newValue);
                }
                catch (FormatException)
                {
                    Console.WriteLine("ENTER A NUMBER!");
                }
                if (databaseManagerClient.ChangeOutputValue(value, tagName))
                {
                    Console.WriteLine("OUTPUT VALUE SUCCESSFULLY CHANGED");
                }
                else
                {
                    Console.WriteLine("OUTPUT VALUE CHANGE FAILED");
                }
                break;
            }
        }

        private static void GetOutputValueScreen(DatabaseManagerClient databaseManagerClient)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("----- GET OUTPUT VALUE SCREEN -----");
            Console.Write("Enter tag name: ");
            string tagName = Console.ReadLine();
            double value = databaseManagerClient.GetOutputValue(tagName);
            if (value != -1000)
            {
                Console.WriteLine($"OUTPUT VALUE: {value}");
            }
            else
            {
                Console.WriteLine("COULD NOT GET OUTPUT VALUE");
            }
        }

        private static void RegisterScreen(AuthenticationClient authenticationClient)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("----- REGISTER SCREEN -----");
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();
            if (authenticationClient.Registration(username, password))
            {
                Console.WriteLine("SUCCESSFULLY REGISTERED");
            }
            else
            {
                Console.WriteLine("REGISTRATION UNSUCCESSFUL");
            }
        }

        private static void Logout(string token, AuthenticationClient authenticationClient)
        {
            if (authenticationClient.Logout(token))
            {
                Console.WriteLine("SUCCESSFULLY LOGGED OUT");
            }
            else
            {
                Console.WriteLine("LOG OUT FAILED!");
            }
        }

        private static void AddDigitalInputTag(DatabaseManagerClient databaseManagerClient)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("----- ADD DIGITAL INPUT TAG SCREEN -----");
            Console.Write("Tag name: ");
            string tagName = Console.ReadLine();
            Console.Write("Description: ");
            string description = Console.ReadLine();
            Console.Write("Driver: ");
            string driver = Console.ReadLine();
            Console.Write("I/O address: ");
            string ioAddress = Console.ReadLine();
            Console.Write("Scan time: ");
            string scanTime = Console.ReadLine();
            Console.Write("On/off scan: ");
            string onOffScan = Console.ReadLine();
            string check = CheckParameters(driver, ioAddress, scanTime, onOffScan);
            if (check == "")
            {
                if (databaseManagerClient.AddDigitalInputTag(tagName, description, driver, 
                    ioAddress, Convert.ToInt32(scanTime), Convert.ToBoolean(onOffScan))) 
                {
                    Console.WriteLine("SUCCESSFULLY ADDED TAG");
                }
                else
                {
                    Console.WriteLine("ERROR! TAG HAS NOT BEEN ADDED");
                }
                
            }
            else
            {
                Console.WriteLine(check);
            }
        }

        private static void AddDigitalOutputTag(DatabaseManagerClient databaseManagerClient)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("----- ADD DIGITAL OUTPUT TAG SCREEN -----");
            Console.Write("Tag name: ");
            string tagName = Console.ReadLine();
            Console.Write("Description: ");
            string description = Console.ReadLine();
            Console.Write("Driver: ");
            string driver = Console.ReadLine();
            Console.Write("I/O address: ");
            string ioAddress = Console.ReadLine();
            Console.Write("Initial value: ");
            string initialValue = Console.ReadLine();
            string check = CheckParameters(driver, ioAddress, initialValue: initialValue);
            if (check == "")
            {
                if (databaseManagerClient.AddDigitalOutputTag(tagName, description, ioAddress, 
                    Convert.ToDouble(initialValue)))
                {
                    Console.WriteLine("SUCCESSFULLY ADDED TAG");
                }
                else
                {
                    Console.WriteLine("ERROR! TAG HAS NOT BEEN ADDED");
                }
            }
            else
            {
                Console.WriteLine(check);
            }
        }

        private static void AddAnalogInputTag(DatabaseManagerClient databaseManagerClient)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("----- ADD ANALOG INPUT TAG SCREEN -----");
            Console.Write("Tag name: ");
            string tagName = Console.ReadLine();
            Console.Write("Description: ");
            string description = Console.ReadLine();
            Console.Write("Driver: ");
            string driver = Console.ReadLine();
            Console.Write("I/O address: ");
            string ioAddress = Console.ReadLine();
            Console.Write("Scan time: ");
            string scanTime = Console.ReadLine();
            Console.Write("On/off scan: ");
            string onOffScan = Console.ReadLine();
            Console.Write("Low limit: ");
            string lowLimit = Console.ReadLine();
            Console.Write("High limit: ");
            string highLimit = Console.ReadLine();
            Console.Write("Units: ");
            string units = Console.ReadLine();
            string check = CheckParameters(driver, ioAddress, scanTime, onOffScan, 
                lowLimit: lowLimit, highLimit: highLimit, units: units);
            if (check == "")
            {
                if (databaseManagerClient.AddAnalogInputTag(tagName, description, driver, ioAddress, 
                    Convert.ToInt32(scanTime), Convert.ToBoolean(onOffScan), Convert.ToDouble(lowLimit), 
                    Convert.ToDouble(highLimit), units))
                {
                    Console.WriteLine("SUCCESSFULLY ADDED TAG");
                }
                else
                {
                    Console.WriteLine("ERROR! TAG HAS NOT BEEN ADDED");
                }
            }
            else
            {
                Console.WriteLine(check);
            }
        }

        private static void AddAnalogOutputTag(DatabaseManagerClient databaseManagerClient)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("----- ADD ANALOG OUTPUT TAG SCREEN -----");
            Console.Write("Tag name: ");
            string tagName = Console.ReadLine();
            Console.Write("Description: ");
            string description = Console.ReadLine();
            Console.Write("Driver: ");
            string driver = Console.ReadLine();
            Console.Write("I/O address: ");
            string ioAddress = Console.ReadLine();
            Console.Write("Initial value: ");
            string initialValue = Console.ReadLine();
            Console.Write("Low limit: ");
            string lowLimit = Console.ReadLine();
            Console.Write("High limit: ");
            string highLimit = Console.ReadLine();
            Console.Write("Units: ");
            string units = Console.ReadLine();
            string check = CheckParameters(driver, ioAddress, initialValue: initialValue,
                lowLimit: lowLimit, highLimit: highLimit, units: units);
            if (check == "")
            {
                if (databaseManagerClient.AddAnalogOutputTag(tagName, description, ioAddress, 
                    Convert.ToDouble(initialValue), Convert.ToDouble(lowLimit), 
                    Convert.ToDouble(highLimit), units))
                {
                    Console.WriteLine("SUCCESSFULLY ADDED TAG");
                }
                else
                {
                    Console.WriteLine("ERROR! TAG HAS NOT BEEN ADDED");
                }
            }
            else
            {
                Console.WriteLine(check);
            }
        }

        private static string CheckParameters(string driver = "-1a!", string ioAddress = "-1a!", 
            string scanTime = "def", string onOffScan = "def", string initialValue = "def", 
            string lowLimit = "def", string highLimit = "def", string units = "-1a!")
        {
            if (driver != "-1a!")
            {
                if (driver.ToLower() != "simulation" && driver.ToLower() != "realtime")
                {
                    return "DRIVER MUST BE ONE OF THE NEXT VALUES: SIMULATION, REALTIME";
                }
            }

            if (ioAddress != "-1a!")
            {
                if (driver.ToLower() == "simulation")
                {
                    if (ioAddress.ToLower() != "s" && ioAddress.ToLower() != "c" && ioAddress.ToLower() != "r")
                    {
                        return "I/O ADDRESS MUST BE ONE OF THE NEXT VALUES: S, C, R";
                    }
                }
            }

            if (scanTime != "def")
            {
                try
                {
                    int scanTimeValue = Convert.ToInt32(scanTime);
                    if (scanTimeValue <= 0)
                    {
                        return "SCAN TIME MUST BE A POSITIVE INTEGER";
                    }
                }
                catch (FormatException)
                {
                    return "SCAN TIME MUST BE A NUMBER";
                }
            }

            if (onOffScan != "def")
            {
                try
                {
                    bool onOffScanValue = Convert.ToBoolean(onOffScan);
                }
                catch (FormatException)
                {
                    return "ON/OFF SCAN VALUE MUST BE A BOOLEAN";
                }
            }

            if (lowLimit != "def")
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
            }

            if (highLimit != "def")
            {
                try
                {
                    int highLimitValue = Convert.ToInt32(highLimit);
                    int lowLimitValue = Convert.ToInt32(lowLimit);
                    if (highLimitValue < 0)
                    {
                        return "LOW LIMIT MUST BE A POSITIVE INTEGER";
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
            }

            if (initialValue != "def")
            {
                try
                {
                    int initial = Convert.ToInt32(initialValue);
                    if (lowLimit == "def")
                    {
                        if (initial != 0 && initial != 1)
                        {
                            return "INITIAL VALUE FOR DIGITAL OUTPUT MUST BE 0 OR 1";
                        }
                    }
                    else
                    {
                        if (initial < Convert.ToInt32(lowLimit) || initial > Convert.ToInt32(highLimit))
                        {
                            return "INITIAL VALUE FOR ANALOG OUTPUT MUST BE BETWEEN THE LOW LIMIT AND HIGH LIMIT";
                        }
                    }
                }
                catch (FormatException)
                {
                    return "INITIAL VALUE MUST BE A NUMBER";
                }
            }

            if (units != "-1a!")
            {
                try
                {
                    int unitsValue = Convert.ToInt32(units);
                    return "UNITS MUST BE A STRING";
                }
                catch (FormatException)
                {
                    return "";
                }
            }

            return "";
        }
    }
}
