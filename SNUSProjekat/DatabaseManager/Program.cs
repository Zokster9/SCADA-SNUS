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
            ServiceReference.AuthenticationClient authenticationClient = 
                new ServiceReference.AuthenticationClient();
            string token = "";
            while (true)
            {
                Console.WriteLine("----- DATABASE MANAGER -----");
                Console.WriteLine("CHOOSE AN OPTION:");
                Console.WriteLine("\t1. Login");
                Console.WriteLine("\t2. Add tag");
                Console.WriteLine("\t3. Remove tag");
                Console.WriteLine("\t4. Turn scan on/off");
                Console.WriteLine("\t5. Change output value");
                Console.WriteLine("\t6. Get output value");
                Console.WriteLine("\t7. Register user");
                Console.WriteLine("\t8. Logout");
                Console.WriteLine("\t9. Exit");
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
                            Console.WriteLine("2");
                            break;
                        case 3:
                            RemoveTagScreen();
                            break;
                        case 4:
                            TurnScanOnOffScreen();
                            break;
                        case 5:
                            ChangeOutputValueScreen();
                            break;
                        case 6:
                            GetOutputValueScreen();
                            break;
                        case 7:
                            RegisterScreen(authenticationClient);
                            break;
                        case 8:
                            Logout(token, authenticationClient);
                            break;
                        case 9:
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

        private static string LoginScreen(ServiceReference.AuthenticationClient authenticationClient)
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

        private static void RemoveTagScreen()
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("----- REMOVE TAG SCREEN -----");
            Console.Write("Enter tag name: ");
            string tagName = Console.ReadLine();
            // PROVERA
            if (true)
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
            // PROVERA
            if (true)
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
                try
                {
                    double value = Convert.ToDouble(newValue);
                }
                catch (FormatException)
                {
                    Console.WriteLine("ENTER A NUMBER!");
                }
                // PROVERA
                if (true)
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
            // PROVERA
            double vrednost = 6;
            if (vrednost != -1000)
            {
                Console.WriteLine($"OUTPUT VALUE: {vrednost}");
            }
            else
            {
                Console.WriteLine("COULD NOT GET OUTPUT VALUE");
            }
        }

        private static void RegisterScreen(ServiceReference.AuthenticationClient authenticationClient)
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

        private static void Logout(string token, ServiceReference.AuthenticationClient authenticationClient)
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
    }
}
