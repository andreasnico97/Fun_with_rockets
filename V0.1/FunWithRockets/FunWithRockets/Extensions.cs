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

        public static void PrintTelemetry(this Vessel vessel, int decimals=3)
        {
            double Round(double n)
            {
                return Math.Round(n, decimals);
            }
            var flight = vessel.Flight();
            var obtFrame = vessel.Orbit.Body.NonRotatingReferenceFrame;
            var srfFrame = vessel.Orbit.Body.ReferenceFrame;

            Console.WriteLine("Altitude above sea: {0}", Round(flight.MeanAltitude));
            Console.WriteLine("Altitude above ground: {0}", Round(flight.SurfaceAltitude));

            Console.WriteLine("Orbit Speed: {0}", Round(vessel.Flight(obtFrame).Speed));
            Console.WriteLine("Surface Speed: {0}", Round(vessel.Flight(srfFrame).Speed));
            Console.WriteLine("Vertical Speed: {0}", Round(vessel.Flight(srfFrame).VerticalSpeed));

            Console.WriteLine("Current throttle: {0}", Round(vessel.Control.Throttle));
            Console.WriteLine("Max thrust: {0}", Round(vessel.AvailableThrust));
            Console.WriteLine("Current thrust: {0}", Round(vessel.Thrust));

            Console.WriteLine("Ship mass: {0}", Round(vessel.Mass));
            Console.WriteLine("Current engine acceleration: {0}", Round(vessel.Thrust / vessel.Mass));
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
                double initialSpeedSquared = Math.Pow(vessel.Flight(srfFrame).Speed, 2);
                float acc = (float)(((3+initialSpeedSquared) / (2 * distanceToGround)) + 9.8);

                float thrustRequired = vessel.Mass * acc;
                float throttle = thrustRequired / (vessel.AvailableThrust);

                if (throttle > 0.98) { shouldThrust = true;  }
                if (throttle < 0.1)  { shouldThrust = false; }

                if(shouldThrust) { vessel.Accelerate(acc); }
                vessel.Control.SASMode =
                    vessel.Flight(srfFrame).Speed < 20 ?
                        SASMode.StabilityAssist : SASMode.Retrograde;

                comOffset = Math.Abs(vessel.BoundingBox(vessel.ReferenceFrame).Item1.Item2);
                distanceToGround = vessel.Flight().SurfaceAltitude - comOffset;
            }

            while(!(vessel.Situation == VesselSituation.Landed ||
                    vessel.Situation == VesselSituation.Splashed))
            {
                vessel.Accelerate(1);
                Console.WriteLine(vessel.Flight(srfFrame).Speed);
            }
            vessel.Accelerate(0);
        }
    }
}
