using System;
using System.Collections.Generic;
using System.Text;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;

namespace FunWithRockets
{
    public class HoverMode
    {
        private readonly Connection _connection;
        public HoverMode(Connection connection)
        {
            _connection = connection;
        }

        public void Activate()
        {
            var spaceCenter = _connection.SpaceCenter();
            var vessel = spaceCenter.ActiveVessel;
            var obtFrame = vessel.Orbit.Body.NonRotatingReferenceFrame;
            var srfFrame = vessel.Orbit.Body.ReferenceFrame;
            var flight = spaceCenter.ActiveVessel.Flight();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Altitude above sea: {0}", Math.Round(flight.MeanAltitude, 3));
                Console.WriteLine("Altitude above ground: {0}", Math.Round(flight.SurfaceAltitude, 3));

                Console.WriteLine("Orbit Speed: {0}", Math.Round(vessel.Flight(obtFrame).Speed, 3));
                Console.WriteLine("Surface Speed: {0}", Math.Round(vessel.Flight(srfFrame).Speed, 3));
                Console.WriteLine("Vertical Speed: {0}",
                    Math.Round(vessel.Flight(srfFrame).VerticalSpeed, 3));

                Console.WriteLine("Current throttle: {0}", Math.Round(vessel.Control.Throttle, 3));
                Console.WriteLine("Max thrust: {0}", Math.Round(vessel.AvailableThrust, 3));
                Console.WriteLine("Current thrust: {0}", Math.Round(vessel.Thrust, 3));

                Console.WriteLine("Ship mass: {0}", Math.Round(vessel.Mass, 3));

                var a = 9.81 - vessel.Flight(srfFrame).VerticalSpeed;
                vessel.Accelerate((float) a);

                Console.WriteLine("Current engine acceleration: {0}", Math.Round((vessel.Thrust / vessel.Mass), 3));

                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
