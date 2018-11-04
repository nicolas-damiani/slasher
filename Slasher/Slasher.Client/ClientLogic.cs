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
            byte[] data = Protocol.GenerateStream(ProtocolConstants.SendType.REQUEST, ProtocolConstants.LOGIN, name);
            clientStream.Write(data, 0, data.Length);

            var response = new byte[ProtocolConstants.DATA_HEADER_SIZE + 8];
            response = Protocol.GetData(TcpClient, ProtocolConstants.DATA_HEADER_SIZE + 8);
            if (response != null)
                return Protocol.checkIfLogged(response);
            return false;

        }

        public string SendMovement(string movement)
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(ProtocolConstants.SendType.REQUEST, ProtocolConstants.MOVEMENT, movement);
            clientStream.Write(data, 0, data.Length);
            return receiveData();
        }

        public string SendAttack(string direction)
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(ProtocolConstants.SendType.REQUEST, ProtocolConstants.ATTACK, direction);
            clientStream.Write(data, 0, data.Length);
            return receiveData();
        }

        public string SendFile(string filePath)
        {
            NetworkStream clientStream = TcpClient.GetStream();
            FileStream sourceFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader binReader = new BinaryReader(sourceFile);
            int parts = unchecked((int)sourceFile.Length) / ProtocolConstants.PART_SIZE;
            int totalRead = 0;
            for (int i = 0; i <= parts; i++)
            {
                byte[] total = Protocol.ReadFully(sourceFile, i, parts, ref totalRead);
                clientStream.Write(total, 0, total.Length);
                sourceFile.Seek(totalRead, SeekOrigin.Begin);
            }
            sourceFile.Close(); 
            binReader.Close();
            return receiveData();
        }

        internal string JoinActiveMatch()
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(ProtocolConstants.SendType.REQUEST, ProtocolConstants.JOIN_MATCH, "");
            clientStream.Write(data, 0, data.Length);
            return receiveData();
        }

        private string receiveData()
        {
            try
            {
                byte[] header = new byte[ProtocolConstants.HEADER_SIZE];
                header = Protocol.GetData(TcpClient, ProtocolConstants.HEADER_SIZE);
                if (header != null)
                    return executeAction(header);
                else
                    throw new SessionEndedException();
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
                case ProtocolConstants.AVATAR_UPLOAD:
                    CheckOkResponse(data);
                    return "Archivo subido correctamente.";
                case ProtocolConstants.SELECT_CHARACTER:
                    CheckOkResponse(data);
                    return "Tipo de jugador asignado correctamente.";
                case ProtocolConstants.JOIN_MATCH:
                    CheckOkResponse(data);
                    return "Unido a partida correctamente correctamente.";
                case ProtocolConstants.MOVEMENT:
                    CheckOkResponse(data);
                    return movementResponse(data);
                case ProtocolConstants.ATTACK:
                    CheckOkResponse(data);
                    return attackResponse(data);
                case ProtocolConstants.END_OF_MATCH:
                    finishActiveMatch(data);
                    return attackResponse(data);
                case ProtocolConstants.ERROR:
                    serverError(data);
                    return "";
                
                default:
                    return "";
            }
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
            if (dataResponse.Equals(ProtocolConstants.OK_RESPONSE_CODE))
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
            if (dataResponse.Contains("500"))
            {
                throw new EndOfMatchException("La partida caducó, debe registrarse a la partida actual para jugar.");
            }
        }

        private void CheckOkResponse(byte[] data)
        {
            string dataResponse = System.Text.Encoding.ASCII.GetString(data);
            if (!dataResponse.Contains(ProtocolConstants.OK_RESPONSE_CODE))
                throw new ClientException("Ocurrió un error inesperado.");
        }

        private void serverError(byte[] data)
        {
            throw new ClientException(Encoding.ASCII.GetString(data));
        }

        internal string SendCharacterType(string characterType)
        {
            NetworkStream clientStream = TcpClient.GetStream();
            byte[] data = Protocol.GenerateStream(ProtocolConstants.SendType.REQUEST, ProtocolConstants.SELECT_CHARACTER, characterType);
            clientStream.Write(data, 0, data.Length);
            return receiveData();
        }
    }
}
