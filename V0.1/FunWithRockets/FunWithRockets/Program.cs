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
            var obtFrame = vessel.Orbit.Body.NonRotatingReferenceFrame;
            var srfFrame = vessel.Orbit.Body.ReferenceFrame;
            var flight = spaceCenter.ActiveVessel.Flight();
            vessel.Control.ActivateNextStage();
            System.Threading.Thread.Sleep(2000);
            vessel.Control.Throttle = 1;
            while (true)
            {
                Console.Clear();
                Console.WriteLine(String.Format("Altitude above sea: {0}", Math.Round(flight.MeanAltitude, 3).ToString()));
                Console.WriteLine(String.Format("Altitude above ground: {0}", Math.Round(flight.SurfaceAltitude, 3).ToString()));
                Console.WriteLine(String.Format("Orbit Speed: {0}", Math.Round(vessel.Flight(obtFrame).Speed, 3).ToString()));
                Console.WriteLine(String.Format("Surface Speed: {0}", Math.Round(vessel.Flight(srfFrame).Speed, 3).ToString()));
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
