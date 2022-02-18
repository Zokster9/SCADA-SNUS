using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trending
{
    class TrendingCallback : ServiceReference.ITrendingCallback
    {
        public void InputTagValueChanged(string tagName, double value)
        {
            Console.WriteLine($"Tag: {tagName} has produced a new value: {value}");
        }
    }
}
