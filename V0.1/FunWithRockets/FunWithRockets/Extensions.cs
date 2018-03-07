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
            var srfFlight = vessel.Flight(vessel.Orbit.Body.ReferenceFrame);
            var obtFlight = vessel.Flight(vessel.Orbit.Body.NonRotatingReferenceFrame);

            Console.WriteLine(   "Altitude above sea: {0}", Round(srfFlight.MeanAltitude));
            Console.WriteLine("Altitude above ground: {0}", Round(srfFlight.SurfaceAltitude));

            Console.WriteLine(     "Orbit Speed: {0}", Round(obtFlight.Speed));
            Console.WriteLine(   "Surface Speed: {0}", Round(srfFlight.Speed));
            Console.WriteLine(  "Vertical Speed: {0}", Round(srfFlight.VerticalSpeed));
            Console.WriteLine("Horizontal Speed. {0}", Round(srfFlight.HorizontalSpeed));

            Console.WriteLine("Current throttle: {0}", Round(vessel.Control.Throttle));
            Console.WriteLine(      "Max thrust: {0}", Round(vessel.AvailableThrust));
            Console.WriteLine(  "Current thrust: {0}", Round(vessel.Thrust));

            Console.WriteLine(                  "Ship mass: {0}", Round(vessel.Mass));
            Console.WriteLine("Current engine acceleration: {0}", Round(vessel.Thrust / vessel.Mass));
        }

        public static string GetOrbitalInfo(this Vessel vessel, bool advanced, int decimals=3)
        {
            double Round(double n)
            {
                return Math.Round(n, decimals);
            }
            var obt = vessel.Orbit;

            var baseString = string.Format("Apoapsis:     {0}\n", Round(obt.ApoapsisAltitude)) +
                             string.Format("Periapsis:    {0}\n", Round(obt.PeriapsisAltitude)) +
                             string.Format("Inclination:  {0}\n", Round(obt.Inclination)) +
                             string.Format("Eccentricity: {0}\n", Round(obt.Eccentricity)) +
                                           "\n" +
                             string.Format("Time to apoapsis:  {0}\n", Round(obt.TimeToApoapsis)) +
                             string.Format("Time to periapsis: {0}\n", Round(obt.TimeToPeriapsis));

            if (!advanced) return baseString;

            // Advanced
            var periodS = obt.Period;
            var period = TimeSpan.FromSeconds(periodS);
            var advancedString = string.Format("Orbital period: {0}\n", period);

            var a3 = Math.Pow(vessel.Orbit.Apoapsis, 3);
            var GM = vessel.Orbit.Body.GravitationalParameter;
            var periodAtApoapsis = 2 * Math.PI * Math.Sqrt(a3 / GM);
            advancedString += string.Format("Peiod if circ. at apoapsis: {0}", periodAtApoapsis);
            
            return baseString + "\n" + advancedString;
        }

        public static void Land(this Vessel vessel)
        {
            var srfFlight = vessel.Flight(vessel.Orbit.Body.ReferenceFrame);

            while (srfFlight.VerticalSpeed > -2)
            {
                System.Threading.Thread.Sleep(100);
            }

            vessel.Control.SAS = true;
            vessel.Control.Gear = true;
            vessel.Control.Brakes = vessel.Orbit.Body.HasAtmosphere;

            while (!(vessel.Control.SASMode == SASMode.Retrograde && vessel.Control.RCS))
            {
                vessel.Control.SASMode = SASMode.Retrograde;
                vessel.Control.RCS = true;
            }

            bool shouldThrust = false;

            double comOffset = Math.Abs(vessel.BoundingBox(vessel.ReferenceFrame).Item1.Item2);
            double distanceToGround = vessel.Flight().SurfaceAltitude - comOffset;
            double srfGrav = vessel.Orbit.Body.SurfaceGravity;

            while (distanceToGround > 0.5)
            {
                double initialSpeedSquared = Math.Pow(srfFlight.Speed, 2);
                float acc = (float)(((3+initialSpeedSquared) / (2 * distanceToGround)) + srfGrav);

                float thrustRequired = vessel.Mass * acc;
                float throttle = thrustRequired / (vessel.AvailableThrust);

                if (throttle > 0.98) { shouldThrust = true;  }
                if (throttle < 0.1)  { shouldThrust = false; }

                if(shouldThrust) { vessel.Accelerate(acc); }
                vessel.Control.SASMode =
                    srfFlight.Speed < 20 ?
                        SASMode.StabilityAssist : SASMode.Retrograde;

                comOffset = Math.Abs(vessel.BoundingBox(vessel.ReferenceFrame).Item1.Item2);
                distanceToGround = vessel.Flight().SurfaceAltitude - comOffset;
            }

            while(!(vessel.Situation == VesselSituation.Landed ||
                    vessel.Situation == VesselSituation.Splashed))
            {
                vessel.Accelerate(1);
            }
            vessel.Accelerate(0);
        }
    }
}
