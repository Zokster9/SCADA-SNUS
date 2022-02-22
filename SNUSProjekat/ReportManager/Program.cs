using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportManager.ServiceReference;

namespace ReportManager
{
    class Program
    {
        static ReportManagerServiceClient client = new ReportManagerServiceClient();
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("----- REPORT MANAGER SCREEN -----");
                Console.WriteLine("\t1. All alarms in a timeframe");
                Console.WriteLine("\t2. All alarms with a certain priority");
                Console.WriteLine("\t3. All tags in a timeframe");
                Console.WriteLine("\t4. Last values of all analog inputs");
                Console.WriteLine("\t5. Last values of all digital inputs");
                Console.WriteLine("\t6. All tag values of a specific tag");
                Console.WriteLine("\t7. Exit");
                Console.Write("ENTER OPTION: ");
                string option = Console.ReadLine();
                try
                {
                    int optionValue = Convert.ToInt32(option);

                    switch (optionValue)
                    {
                        case 1:
                            Report1Screen();
                            break;
                        case 2:
                            Report2Screen();
                            break;
                        case 3:
                            Report3Screen();
                            break;
                        case 4:
                            Report4Screen();
                            break;
                        case 5:
                            Report5Screen();
                            break;
                        case 6:
                            Report6Screen();
                            break;
                        case 7:
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

        private static void Report6Screen()
        {
            Console.WriteLine("----- ALL TAG VALUES OF A SPECIFIC TAG -----");
            Console.Write("Tag name: ");
            string tagName = Console.ReadLine();
            TagValue[] tagValues = client.GetAllTagValues(tagName);
            Console.WriteLine();
            foreach (TagValue tagValue in tagValues)
            {
                Console.WriteLine($"Tag name: {tagValue.TagName}; Value: {tagValue.Value}; " +
                    $"Type: {tagValue.Type}; Time: {tagValue.ValueTime}");
            }
            Console.WriteLine();
        }

        private static void Report5Screen()
        {
            TagValue[] tagValues = client.GetLastDigitalInputTagValue();
            Console.WriteLine();
            Console.WriteLine("----- LAST DIGITAL INPUT TAG VALUES -----");
            foreach (TagValue tagValue in tagValues)
            {
                Console.WriteLine($"Tag name: {tagValue.TagName}; Value: {tagValue.Value}; " +
                    $"Type: {tagValue.Type}; Time: {tagValue.ValueTime}");
            }
            Console.WriteLine();
        }

        private static void Report4Screen()
        {
            TagValue[] tagValues = client.GetLastAnalogInputTagValue();
            Console.WriteLine();
            Console.WriteLine("----- LAST ANALOG INPUT TAG VALUES -----");
            foreach (TagValue tagValue in tagValues)
            {
                Console.WriteLine($"Tag name: {tagValue.TagName}; Value: {tagValue.Value}; " +
                    $"Type: {tagValue.Type}; Time: {tagValue.ValueTime}");
            }
            Console.WriteLine();
        }

        private static void Report3Screen()
        {
            Console.WriteLine("----- ALL TAG VALUES IN A TIMEFRAME -----");
            Console.Write("Enter starting timeframe(format: dd-MMM-y HH:mm:ss): ");
            string startingTimeframe = Console.ReadLine();
            Console.Write("Enter ending timeframe(format: dd-MMM-y HH:mm:ss): ");
            string endingTimeframe = Console.ReadLine();
            DateTime startingTime;
            DateTime endingTime;
            try
            {
                startingTime = DateTime.ParseExact(startingTimeframe, "dd-MMM-y HH:mm:ss",
                    CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine("STARTING TIMEFRAME IS IN THE WRONG FORMAT!");
                return;
            }
            try
            {
                endingTime = DateTime.ParseExact(endingTimeframe, "dd-MMM-y HH:mm:ss",
                    CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine("STARTING TIMEFRAME IS IN THE WRONG FORMAT!");
                return;
            }
            TagValue[] tagValues = client.GetAllTagValuesByTime(startingTime, endingTime);
            Console.WriteLine();
            foreach (TagValue tagValue in tagValues)
            {
                Console.WriteLine($"Tag name: {tagValue.TagName}; Value: {tagValue.Value}; " +
                    $"Type: {tagValue.Type}; Time: {tagValue.ValueTime}");
            }
            Console.WriteLine();
        }

        private static void Report2Screen()
        {
            Console.WriteLine("----- ALARMS WITH A CERTAIN PRIORITY -----");
            Console.Write("Priority: ");
            string priority = Console.ReadLine();
            if (CheckPriority(priority))
            {
                TriggeredAlarm[] triggeredAlarms = client.GetAllTriggeredAlarmsByPriority(Convert.ToInt32(priority));
                Console.WriteLine();
                foreach (TriggeredAlarm alarm in triggeredAlarms)
                {
                    Console.WriteLine($"Type: {alarm.Type}; Priority: {alarm.Priority}; " +
                        $"Limit: {alarm.Limit}; Tag name: {alarm.TagName}; Time: {alarm.TriggerTime}");
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("PRIORITY VALUE MUST BE 1, 2, OR 3");
            }   
        }

        private static void Report1Screen()
        {
            Console.WriteLine("----- ALL ALARMS IN A TIMEFRAME -----");
            Console.Write("Enter starting timeframe(format: dd-MMM-y HH:mm:ss): ");
            string startingTimeframe = Console.ReadLine();
            Console.Write("Enter ending timeframe(format: dd-MMM-y HH:mm:ss): ");
            string endingTimeframe = Console.ReadLine();
            DateTime startingTime;
            DateTime endingTime;
            try
            {
                startingTime = DateTime.ParseExact(startingTimeframe, "dd-MMM-y HH:mm:ss", 
                    CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine("STARTING TIMEFRAME IS IN THE WRONG FORMAT!");
                return;
            }
            try
            {
                endingTime = DateTime.ParseExact(endingTimeframe, "dd-MMM-y HH:mm:ss",
                    CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine("STARTING TIMEFRAME IS IN THE WRONG FORMAT!");
                return;
            }
            TriggeredAlarm[] triggeredAlarms = client.GetAllTriggeredAlarmsByTime(startingTime, endingTime);
            Console.WriteLine();
            foreach(TriggeredAlarm alarm in triggeredAlarms)
            {
                Console.WriteLine($"Type: {alarm.Type}; Priority: {alarm.Priority}; " +
                    $"Limit: {alarm.Limit}; Tag name: {alarm.TagName}; Time: {alarm.TriggerTime}");
            }
            Console.WriteLine();
        }

        private static bool CheckPriority(string priority)
        {
            try
            {
                int priorityVal = Convert.ToInt32(priority);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
