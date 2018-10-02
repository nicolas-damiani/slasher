using Slasher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Slasher.Server
{
    public class ServerInterface
    {
        private ServerLogic ServerLogic { get; set; }


        public ServerInterface()
        {
            ServerLogic = new ServerLogic();
        }

        public void Start()
        {
            Console.WriteLine("Bienvenido al sistema Slasher Servidor!");

            Thread acceptConnections = new Thread(() => ServerLogic.ConnectServer());
            acceptConnections.Start();

            string exitCommand = Console.ReadLine();
            ServerLogic.ShutDownConnections();
            Environment.Exit(0);
            //ServerLogic.ShutDownConnections();
        }
    }
}
