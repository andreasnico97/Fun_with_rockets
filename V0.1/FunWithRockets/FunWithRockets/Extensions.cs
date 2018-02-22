using System;
using KRPC.Client.Services.SpaceCenter;

namespace FunWithRockets
{
    public static class Extensions
    {
        public static void Accelerate(this Vessel vessel, float a)
        {
            float f = vessel.Mass * a;
            float thr = f / (vessel.AvailableThrust);
            vessel.Control.Throttle = thr;
        }

        public static void PrintTelemetry(this Vessel vessel)
        {
            var flight = vessel.Flight();
            var obtFrame = vessel.Orbit.Body.NonRotatingReferenceFrame;
            var srfFrame = vessel.Orbit.Body.ReferenceFrame;

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
            Console.WriteLine("Current engine acceleration: {0}", Math.Round((vessel.Thrust / vessel.Mass), 3));
        }

        public static void Land(this Vessel vessel)
        {
            var srfFrame = vessel.Orbit.Body.ReferenceFrame;

            while (vessel.Flight(srfFrame).VerticalSpeed > -2)
            {
                System.Threading.Thread.Sleep(100);
            }

            vessel.Control.SAS = true;
            vessel.Control.Brakes = true;
            vessel.Control.Gear = true;
            while (!(vessel.Control.SASMode == SASMode.Retrograde && vessel.Control.RCS))
            {
                vessel.Control.SASMode = SASMode.Retrograde;
                vessel.Control.RCS = true;
            }

            bool shouldThrust = false;

            double comOffset = Math.Abs(vessel.BoundingBox(vessel.ReferenceFrame).Item1.Item2);
            double distanceToGround = vessel.Flight().SurfaceAltitude - comOffset;

            while (distanceToGround > 0.5)
            {
                comOffset = Math.Abs(vessel.BoundingBox(vessel.ReferenceFrame).Item1.Item2);
                distanceToGround = vessel.Flight().SurfaceAltitude - comOffset;

                double initialSpeedSquared = Math.Pow(vessel.Flight(srfFrame).Speed, 2);
                float a = (float)( ((3+initialSpeedSquared) / (2 * distanceToGround)) + 9.8);

                float thrustRequired = vessel.Mass * a;
                float throttle = thrustRequired / (vessel.AvailableThrust);

                if (vessel.Flight(srfFrame).Speed < 20) { vessel.Control.SASMode = SASMode.StabilityAssist; }

                if (throttle > 0.98) { shouldThrust = true;  }
                if (throttle < 0.1)  { shouldThrust = false; }
                
                if(shouldThrust) { vessel.Accelerate(a); }
            }
            while(!(vessel.Situation == VesselSituation.Landed || vessel.Situation == VesselSituation.Splashed))
            {
                vessel.Accelerate((float)1);
                Console.WriteLine(vessel.Flight(srfFrame).Speed);
            }
            vessel.Accelerate(0);
            
        }
    }
}
