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

        public string SendMovement(string movement)
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "40", movement);
            clientStream.Write(data, 0, data.Length);
            return receiveData();
        }

        public string SendFile(string filePath)
        {
            byte[] stream = Protocol.ReadFully(filePath);
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "10", System.Text.Encoding.ASCII.GetString(stream));
            clientStream.Write(data, 0, data.Length);
            return receiveData();
        }

        internal string CheckGameStatus()
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "30", "");
            clientStream.Write(data, 0, data.Length);
            return receiveData();
        }

        internal string JoinActiveMatch()
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "20", "");
            clientStream.Write(data, 0, data.Length);
            return receiveData();
        }

        private string receiveData()
        {
            try
            {
                byte[] header = new byte[Protocol.HEADER_SIZE + 5];
                header = Protocol.GetData(TcpClient, Protocol.HEADER_SIZE + 5);
                if (header != null)
                    return executeAction(header);
                else
                    return "Ocurrio un error inesperado";
            }
            catch (SocketException ex)
            {
                throw new ClientException(ex.Message);
            }
        }

        private string executeAction(byte[] header)
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
                    return "Archivo subido correctamente.";
                case 16:
                    CheckOkResponse(data);
                    return "Tipo de jugador asignado correctamente.";
                case 21:
                    CheckOkResponse(data);
                    return "Unido a partida correctamente correctamente.";
                case 31:
                    CheckOkResponse(data);
                    break;
                case 41:
                    CheckOkResponse(data);
                    return movementResponse(data);
                case 51:
                    CheckOkResponse(data);
                    return movementResponse(data);
                case 61:
                    finishActiveMatch(data);
                    return attackResponse(data);
                case 99:
                    serverError(data);
                    return "";
                
                default:
                    return "";
            }
            return "";
        }

        private string movementResponse(byte[] data)
        {
            string dataResponse = System.Text.Encoding.ASCII.GetString(data);
            string infoOfMovement = dataResponse.Split('|')[1];
            return infoOfMovement;
        }

        private string attackResponse(byte[] data)
        {
            string dataResponse = System.Text.Encoding.ASCII.GetString(data);
            string infoOfMovement = dataResponse.Split('|')[1];
            return infoOfMovement;
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
            if (dataResponse.Contains("400"))
            {
                string winners = dataResponse.Split('|')[1];
                throw new EndOfMatchException("Partida finalizada, gana monstruo: " + winners);
            }
        }

        private void CheckOkResponse(byte[] data)
        {
            string dataResponse = System.Text.Encoding.ASCII.GetString(data);
            if (!dataResponse.Contains(Protocol.OkResponse))
                throw new ClientException("Ocurrió un error inesperado.");
        }

        private void serverError(byte[] data)
        {
            throw new ClientException(Encoding.ASCII.GetString(data));
        }

        internal string SendCharacterType(string characterType)
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(Protocol.SendType.REQUEST, "15", characterType);
            clientStream.Write(data, 0, data.Length);
            return receiveData();
        }
    }
}
