using System;
using System.Threading;
using KRPC.Client.Services.SpaceCenter;

namespace FunWithRockets
{
    public class HoverMode
    {
        private bool _activated = false;
        private Vessel _vessel;
        public HoverMode(Vessel vessel)
        {
            _vessel = vessel;
        }

        public void Activate()
        {
            _activated = true;
            var thread = new Thread(Hover);
            thread.Start();
        }

        public void Deactivate()
        {
            _activated = false;
            _vessel.Accelerate(0);
        }

        private void Hover()
        {
            var srfFrame = _vessel.Orbit.Body.ReferenceFrame;
            while (_activated)
            {
                Console.Clear();
                var a = 9.81 - _vessel.Flight(srfFrame).VerticalSpeed;
                _vessel.Accelerate((float)a);
            }
        }

    }
}
