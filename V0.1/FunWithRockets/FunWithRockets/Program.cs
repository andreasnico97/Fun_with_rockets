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
            Console.WriteLine("Buckle up");
            vessel.Land();
            Console.WriteLine("Landed");
        }
    }
}
