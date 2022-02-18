using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNUSProjekat
{
    public class SimulationDriver
    {
        public static double ReturnValue(string address)
        {
            if (address == "S") return Sine();
            else if (address == "C") return Cosine();
            else if (address == "R") return Ramp();
            else return -1000;
        }

        private static double Sine()
        {
            return 100 * Math.Sin((double)DateTime.Now.Second / 60 * Math.PI);
        }

        private static double Cosine()
        {
            return 100 * Math.Cos((double)DateTime.Now.Second / 60 * Math.PI);
        }

        private static double Ramp()
        {
            return 100 * DateTime.Now.Second / 60;
        }
    }
}