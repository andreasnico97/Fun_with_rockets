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
            var hoverMode = new HoverMode(connection);
            hoverMode.Activate();
        }
    }
}
