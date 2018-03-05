using System;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;

namespace FunWithRockets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var connection = new Connection();
            var vessel = connection.SpaceCenter().ActiveVessel;
            vessel.PrintTelemetry();
            Console.WriteLine("Buckle up");
            vessel.Land();
            Console.WriteLine("Landed");
            vessel.PrintTelemetry();
        }
    }
}
