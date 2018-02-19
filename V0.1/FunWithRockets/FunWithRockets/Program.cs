using System;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;

namespace FunWithRockets
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new Connection();
            var spaceCenter = connection.SpaceCenter();
            var vessel = spaceCenter.ActiveVessel;
            var refFrame = vessel.Orbit.Body.ReferenceFrame;
            while (true)
                Console.WriteLine(vessel.Position(refFrame));
        }
    }
}
