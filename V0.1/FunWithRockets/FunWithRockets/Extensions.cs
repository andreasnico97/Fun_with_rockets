﻿using System;
using System.Collections.Generic;
using System.Text;
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
            var obtFrame = vessel.Orbit.Body.NonRotatingReferenceFrame;

            while (vessel.Flight(srfFrame).VerticalSpeed > -2)
            {
                System.Threading.Thread.Sleep(100);
            }

            vessel.Control.SAS = true;
            vessel.Control.RCS = true;
            vessel.Control.Brakes = true;
            vessel.Control.SASMode = SASMode.Retrograde;
            bool thrust = false;

            while (!(vessel.Situation.Equals(VesselSituation.Landed) || vessel.Situation.Equals(VesselSituation.Splashed)))
            {
                var com_offset = Math.Abs(vessel.BoundingBox(vessel.ReferenceFrame).Item1.Item2);

                var v_isq = Math.Pow(vessel.Flight(srfFrame).Speed, 2);
                var d = vessel.Flight().SurfaceAltitude - com_offset;
                var a = (float)(((v_isq) / (2 * d)) + 9.81);

                float f = vessel.Mass * a;
                float thr = f / (vessel.AvailableThrust);
                if (thr > 0.98)
                {
                    thrust = true;
                }

                if(thrust)
                {
                    vessel.Accelerate(a);
                }       
            }
            vessel.Accelerate(0);
            
        }
    }
}
