using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNUSProjekat
{
    public class RealTimeDriver
    {
        public static Dictionary<string, int> values = new Dictionary<string, int>();

        public static double ReturnValue(string address)
        {
            try
            {
                return values[address];
            }
            catch
            {
                return -1000;
            }
        }
    }
}