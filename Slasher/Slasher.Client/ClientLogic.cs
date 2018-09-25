using Protocols;
using Slasher.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Client
{
    public class ClientLogic
    {

        public TcpClient TcpClient { get; set; }
        private bool Connected { get; set; }



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

            var response = new byte[Protocol.HEADER_SIZE + 8];
            response = Protocol.GetData(TcpClient, Protocol.HEADER_SIZE + 8);
            if (response != null)
                return Protocol.checkIfLogged(response);
            return false;

        }

        public void SendMovement(string movement)
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "10", movement);
            clientStream.Write(data, 0, data.Length);

            var response = new byte[Protocol.HEADER_SIZE + 8];
            response = Protocol.GetData(TcpClient, Protocol.HEADER_SIZE + 8);
            /*    if (response != null)
                    return Protocol.checkIfLogged(response);
                return false;*/
        }

        public bool SendFile(string filePath)
        {
            byte[] stream = Protocol.ReadFully(filePath);
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "20", System.Text.Encoding.ASCII.GetString(stream));
            
            clientStream.Write(data, 0, data.Length);

            var response = new byte[Protocol.HEADER_SIZE + 8];
            response = Protocol.GetData(TcpClient, Protocol.HEADER_SIZE + 8);
            if (response != null)
                return Protocol.checkIfLogged(response);
            return false;
        }

        internal bool CheckGameStatus()
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "40", "");
            clientStream.Write(data, 0, data.Length);

            var response = new byte[Protocol.HEADER_SIZE + 8];
            response = Protocol.GetData(TcpClient, Protocol.HEADER_SIZE + 8);
            if (response != null)
                return Protocol.checkIfMatchFinished(response);
            return false;
        }

        internal bool JoinActiveMatch()
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "35", "");
            clientStream.Write(data, 0, data.Length);

            var response = new byte[Protocol.HEADER_SIZE + 8];
            response = Protocol.GetData(TcpClient, Protocol.HEADER_SIZE + 8);
            if (response != null)
                return Protocol.checkIfJoinedToMatch(response);
            return false;
        }
    }
}
