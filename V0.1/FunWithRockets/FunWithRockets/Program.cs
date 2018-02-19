using System;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;

namespace FunWithRockets
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new Connection())
            {
                var spaceCenter = connection.SpaceCenter();
                var vessel = spaceCenter.ActiveVessel;
                vessel.Name = "My Vessel";
                var flightInfo = vessel.Flight();
                Console.WriteLine(flightInfo.MeanAltitude);
                Console.WriteLine("test");
            }
        }
    }
}
