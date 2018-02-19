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
            var srfFrame = vessel.Orbit.Body.ReferenceFrame;

            while (true)
            {
                Console.Clear();
                var a = 9.81 - vessel.Flight(srfFrame).VerticalSpeed;
                vessel.Accelerate((float) a);
                vessel.PrintTelemetry();
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
