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
        static AuthenticationClient authenticationClient =
                new AuthenticationClient();
        static DatabaseManagerClient databaseManagerClient =
            new DatabaseManagerClient();
        static void Main(string[] args)
        {
            databaseManagerClient.LoadScadaConfig();
            MainScreen();
        }

        private static void MainScreen()
        {
            while (true)
            {
                Console.WriteLine("----- LOGIN SCREEN -----");
                Console.WriteLine("CHOOSE AN OPTION:");
                Console.WriteLine("\t1. Login");
                Console.WriteLine("\t2. Register user");
                Console.WriteLine("\t3. Exit");
                Console.Write("ENTER AN OPTION: ");
                string option = Console.ReadLine();
                try
                {
                    int optionValue = Convert.ToInt32(option);

                    switch (optionValue)
                    {
                        case 1:
                            string token = LoginScreen();
                            DatabaseManagerScreen(token);
                            break;
                        case 2:
                            RegisterScreen();
                            break;
                        case 3:
                            Environment.Exit(0);
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("WRONG INPUT");
                }
            }
        }

        private static void DatabaseManagerScreen(string token)
        {
            while (true)
            {
                Console.WriteLine("----- DATABASE MANAGER -----");
                Console.WriteLine("CHOOSE AN OPTION:");
                Console.WriteLine("\t1. Add tag");
                Console.WriteLine("\t2. Remove tag");
                Console.WriteLine("\t3. Add alarm");
                Console.WriteLine("\t4. Remove alarm");
                Console.WriteLine("\t5. Turn scan on/off");
                Console.WriteLine("\t6. Change output value");
                Console.WriteLine("\t7. Get output value");
                Console.WriteLine("\t8. Logout");
                Console.Write("ENTER AN OPTION: ");
                string option = Console.ReadLine();
                try
                {
                    int optionValue = Convert.ToInt32(option);

                    switch (optionValue)
                    {
                        case 1:
                            AddTagScreen();
                            break;
                        case 2:
                            RemoveTagScreen();
                            break;
                        case 3:
                            AddAlarm();
                            break;
                        case 4:
                            RemoveAlarm();
                            break;
                        case 5:
                            TurnScanOnOffScreen();
                            break;
                        case 6:
                            ChangeOutputValueScreen();
                            break;
                        case 7:
                            GetOutputValueScreen();
                            break;
                        case 8:
                            Logout(token);
                            MainScreen();
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("WRONG INPUT");
                }
            }
        }

        private static void RemoveAlarm()
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
                if (databaseManagerClient.RemoveAlarm(tagName, type.ToLower(), 
                    Convert.ToInt32(priority), Convert.ToDouble(limit)))
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

        private static void AddAlarm()
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
                if (databaseManagerClient.AddAlarm(tagName, type.ToLower(), 
                    Convert.ToInt32(priority), Convert.ToDouble(limit)))
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
            catch
            {
                return "PRIORITY MUST BE A NUMBER";
            }
            try
            {
                double limitValue = Convert.ToDouble(limit);
            }
            catch
            {
                return "LIMIT MUST BE A NUMBER";
            }
            return "";
        }

        private static string LoginScreen()
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

        private static void AddTagScreen()
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
                            AddDigitalInputTag();
                            break;
                        case 2:
                            AddDigitalOutputTag();
                            break;
                        case 3:
                            AddAnalogInputTag();
                            break;
                        case 4:
                            AddAnalogOutputTag();
                            break;
                        case 5:
                            exit = true;
                            break;
                    }
                    if (exit) break;
                }
                catch
                {
                    Console.WriteLine("WRONG INPUT");
                }
            }
        }

        private static void RemoveTagScreen()
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

        private static void TurnScanOnOffScreen()
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

        private static void ChangeOutputValueScreen()
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
                catch
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

        private static void GetOutputValueScreen()
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

        private static void RegisterScreen()
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

        private static void Logout(string token)
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

        private static void AddDigitalInputTag()
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

        private static void AddDigitalOutputTag()
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

        private static void AddAnalogInputTag()
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

        private static void AddAnalogOutputTag()
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
                catch
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
                catch
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
                catch
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
                catch
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
                catch
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
                catch
                {
                    return "";
                }
            }

            return "";
        }
    }
}
