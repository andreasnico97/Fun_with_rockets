using System;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;

namespace FunWithRockets
{
    class Program
    {
        static void accelerate(Vessel vessel, float a)
        {
            float f = vessel.Mass * a;
            float thr = f / (vessel.AvailableThrust);
            vessel.Control.Throttle = thr;
        }
        static void Main(string[] args)
        {
            var connection = new Connection();
            var spaceCenter = connection.SpaceCenter();
            var vessel = spaceCenter.ActiveVessel;
            var obtFrame = vessel.Orbit.Body.NonRotatingReferenceFrame;
            var srfFrame = vessel.Orbit.Body.ReferenceFrame;
            var flight = spaceCenter.ActiveVessel.Flight();
            while (true)
            {
                Console.Clear();
                Console.WriteLine(String.Format("Altitude above sea: {0}", Math.Round(flight.MeanAltitude, 3).ToString()));
                Console.WriteLine(String.Format("Altitude above ground: {0}", Math.Round(flight.SurfaceAltitude, 3).ToString()));

                Console.WriteLine(String.Format("Orbit Speed: {0}", Math.Round(vessel.Flight(obtFrame).Speed, 3).ToString()));
                Console.WriteLine(String.Format("Surface Speed: {0}", Math.Round(vessel.Flight(srfFrame).Speed, 3).ToString()));
                Console.WriteLine(String.Format("Vertical Speed: {0}", Math.Round(vessel.Flight(srfFrame).VerticalSpeed, 3).ToString()));

                Console.WriteLine(String.Format("Current throttle: {0}", Math.Round(vessel.Control.Throttle, 3).ToString()));
                Console.WriteLine(String.Format("Max thrust: {0}", Math.Round(vessel.AvailableThrust, 3).ToString()));
                Console.WriteLine(String.Format("Current thrust: {0}", Math.Round(vessel.Thrust, 3).ToString()));

                Console.WriteLine(String.Format("Ship mass: {0}", Math.Round(vessel.Mass, 3).ToString()));

                var a = 9.81 - ((vessel.Flight(srfFrame).VerticalSpeed));
                accelerate(vessel, (float)a);

                Console.WriteLine(string.Format("Current engine acceleration: {0}", Math.Round((vessel.Thrust/vessel.Mass), 3)));

                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
