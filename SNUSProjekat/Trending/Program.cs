using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trending.ServiceReference;
using System.ServiceModel;

namespace Trending
{
    class Program
    {
        static TrendingClient trendingClient = new TrendingClient(
            new InstanceContext(new TrendingCallback()));
        static void Main(string[] args)
        {
            Console.WriteLine("Trending started...");
            trendingClient.Init();
            Console.ReadKey();
        }
    }
}
