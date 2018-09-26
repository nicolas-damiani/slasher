using Exceptions;
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
        public bool Connected { get; set; }
        public bool InActiveMatch { get; set; }

        public ClientLogic()
        {
            Connected = true;
            InActiveMatch = false;
        }

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
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "40", movement);
            clientStream.Write(data, 0, data.Length);
            receiveData();
        }

        public void SendFile(string filePath)
        {
            byte[] stream = Protocol.ReadFully(filePath);
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "10", System.Text.Encoding.ASCII.GetString(stream));
            clientStream.Write(data, 0, data.Length);
            receiveData();
        }

        internal void CheckGameStatus()
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "30", "");
            clientStream.Write(data, 0, data.Length);
            receiveData();
        }

        internal void JoinActiveMatch()
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "20", "");
            clientStream.Write(data, 0, data.Length);
            receiveData();
        }

        private void receiveData()
        {
            try
            {
                byte[] header = new byte[Protocol.HEADER_SIZE + 5];
                header = Protocol.GetData(TcpClient, Protocol.HEADER_SIZE + 5);
                if (header != null)
                    executeAction(header);
            }
            catch (SocketException ex)
            {
                throw new ClientException(ex.Message);
            }
        }

        private void executeAction(byte[] header)
        {
            NetworkStream nws = TcpClient.GetStream();
            int dataLength = Protocol.GetDataLength(header);
            byte[] data = new byte[dataLength];
            data = Protocol.GetData(TcpClient, dataLength);
            int action = Protocol.getCommandAction(header);
            switch (action)
            {
                case 11:
                    CheckOkResponse(data);
                    break;
                case 21:
                    CheckOkResponse(data);
                    break;
                case 31:
                    CheckOkResponse(data);
                    break;
                case 41:
                    CheckOkResponse(data);
                    break;
                case 51:
                    //attack
                    break;
                case 61:
                    finishActiveMatch(data);
                    break;

                case 99:
                    serverError(data);
                    break;

                default:
                    string hola = System.Text.Encoding.ASCII.GetString(header);
                    Console.WriteLine("cliente no conectado es: " + hola);
                    break;
            }
        }

        private void finishActiveMatch(byte [] data)
        {
            InActiveMatch = false;
            string dataResponse = System.Text.Encoding.ASCII.GetString(data);
            if (dataResponse.Equals("200"))
                throw new EndOfMatchException("Partida finalizada, no hubieron ganadores.");
            if (dataResponse.Contains("300"))
            {
                string winners = dataResponse.Split('|')[1];
                throw new EndOfMatchException("Partida finalizada, ganan sobrevivientes. Ganadores: "+winners);
            }
        }

        private void CheckOkResponse(byte[] data)
        {
            string dataResponse = System.Text.Encoding.ASCII.GetString(data);
            if (!dataResponse.Equals(Protocol.OkResponse))
                throw new ClientException("Ocurrió un error inesperado.");
        }

        private void serverError(byte[] data)
        {
            throw new ClientException(Encoding.ASCII.GetString(data));
        }
    }
}
