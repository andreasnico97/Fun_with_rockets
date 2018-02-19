using System;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;

namespace FunWithRockets
{
    public class Program
    {
        static void Main(string[] args)
        {
            var connection = new Connection();
            var vessel = connection.SpaceCenter().ActiveVessel;
            var hoverMode = new HoverMode(vessel);
            hoverMode.Activate();
            System.Threading.Thread.Sleep(20000);
            hoverMode.Deactivate();
        }
    }
}
