using Protocols;
using Slasher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Client
{
    public class ClientLogic
    {

        public TcpClient TcpClient { get; set; }



        public bool connect(string name, string serverIP, int port)
        {
            if (makeConnection(name, serverIP, port))
            {
                return true;
            }
            return false;
        }

        private bool makeConnection(string name, string serverIP, int port)
        {
            TcpClient = new TcpClient(serverIP, port);
            return authenticateClient(name);

        }

        private bool authenticateClient(string name)
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "01", name);
            clientStream.Write(data, 0, data.Length);

            var response = new byte[12];
            response = Protocol.GetData(TcpClient, 12);
            if (response != null)
                return Protocol.checkIfLogged(response);
            return false;

        }


    }
}
