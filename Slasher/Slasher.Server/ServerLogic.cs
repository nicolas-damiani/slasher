using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slasher.Entities;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Protocols;
using System.IO;
using Exceptions;
using System.Configuration;

namespace Slasher.Server
{
    class ServerLogic
    {
        public Match Match { get; set; }
        private List<TcpClient> TcpClients { get; set; }
        private Dictionary<TcpClient, User> RegisteredUsers { get; set; }
        public bool Connected { get; set; }
        private static string startupPath = "Archivos";
        private TcpListener Listener;
        private object registrationLock;


        public ServerLogic()
        {
            TcpClients = new List<TcpClient>();
            Match = new Match();
            RegisteredUsers = new Dictionary<TcpClient, User>();
            registrationLock = new object();
            Match = new Match();
            Match.StartMatch();

        }

        internal void ShutDownConnections()
        {
            Listener.Stop();
            foreach (KeyValuePair<TcpClient, User> entry in RegisteredUsers)
            {
                entry.Key.GetStream().Close();
                entry.Key.Close();
                entry.Value.Connected = false;
            }
            Connected = false;
        }

        internal void ConnectServer()
        {
            string ipString = ConfigurationManager.AppSettings["IpConfiguration"];
            int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            IPAddress ipAddress = IPAddress.Parse(ipString);
            Listener = new TcpListener(ipAddress, port);
            Connected = true;
            Console.WriteLine("local ip address: " + ipAddress);
            Listener.Start();
            while (Connected)
            {
                try
                {
                    TcpClient tcpClient = Listener.AcceptTcpClient();
                    TcpClients.Add(tcpClient);
                    Thread clientThread = new Thread(() => receiveCommands(tcpClient));
                    clientThread.Start();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void receiveCommands(TcpClient client)
        {
            User user = new User();
            while (user.Connected)
            {
                byte[] headerInformation = new byte[ProtocolConstants.HEADER_SIZE];
                headerInformation = Protocol.GetData(client, ProtocolConstants.HEADER_SIZE);
                if (headerInformation != null)
                    executeCommand(headerInformation, client, ref user);
                else
                {
                    user.Connected = false;
                    showRegisteredPlayers();
                    showConnectedPlayers();
                }
            }
        }

        private void showConnectedPlayers()
        {
            string connectedPlayers = "Usuarios conectados: ";
            foreach (KeyValuePair<TcpClient, User> entry in RegisteredUsers)
            {
                if (entry.Value.Connected)
                {
                    connectedPlayers += entry.Value.NickName + " \n";
                }
            }
            Console.WriteLine(connectedPlayers);
        }


        private void showRegisteredPlayers()
        {
            string connectedPlayers = "Usuarios registrados: ";
            foreach (KeyValuePair<TcpClient, User> entry in RegisteredUsers)
            {
                connectedPlayers += entry.Value.NickName + " \n";
            }
            Console.WriteLine(connectedPlayers);
        }

        private void executeCommand(byte[] headerInformation, TcpClient client, ref User user)
        {
            NetworkStream nws = client.GetStream();
            try
            {
                int dataLength = Protocol.GetDataLength(headerInformation);
                int command = Protocol.getCommandAction(headerInformation);

                switch (command)
                {
                    case ProtocolConstants.LOGIN:
                        {
                            byte[] data = new byte[dataLength];
                            data = Protocol.GetData(client, dataLength);
                            string name = UnicodeEncoding.ASCII.GetString(data);
                            user.NickName = name;
                            sendAuthorizatonData(name, nws, client, ref user);
                            break;
                        }
                    case ProtocolConstants.AVATAR_UPLOAD:
                        byte[] partsInfoData = Protocol.GetData(client, 2);
                        string amountOfPartsString = UnicodeEncoding.ASCII.GetString(partsInfoData);
                        int amountOfParts = Int32.Parse(amountOfPartsString);
                        downloadFile(RegisteredUsers[client].NickName, dataLength, amountOfParts, client);
                        sendFileResponse(nws);
                        break;
                    case ProtocolConstants.SELECT_CHARACTER:
                        {
                            byte[] data = new byte[dataLength];
                            data = Protocol.GetData(client, dataLength);
                            selectCharacterType(RegisteredUsers[client], nws, data);
                            break;
                        }
                    case ProtocolConstants.JOIN_MATCH:
                        joinMatch(RegisteredUsers[client], nws);
                        break;
                    case ProtocolConstants.MOVEMENT:
                        {
                            byte[] data = new byte[dataLength];
                            data = Protocol.GetData(client, dataLength);
                            string movement = UnicodeEncoding.ASCII.GetString(data);
                            playerAction(nws, RegisteredUsers[client], movement, ActionType.MOVEMENT);
                            break;
                        }
                    case ProtocolConstants.ATTACK:
                        {
                            byte[] data = new byte[dataLength];
                            data = Protocol.GetData(client, dataLength);
                            string attackDirection = UnicodeEncoding.ASCII.GetString(data);
                            playerAction(nws, RegisteredUsers[client], attackDirection, ActionType.ATTACK);
                            break;
                        }
                };
            }
            catch (Exception)
            {
                sendError(nws, "Ocurrió un error inesperado.");
            }
        }

        private void selectCharacterType(User user, NetworkStream nws, byte[] data)
        {
            string command = UnicodeEncoding.ASCII.GetString(data);
            if (user.CharacterTypesCommand.ContainsKey(command))
            {
                user.SetCharacterType(user.CharacterTypesCommand[command]);
                byte[] responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.SELECT_CHARACTER, ProtocolConstants.OK_RESPONSE_CODE);
                nws.Write(responseStream, 0, responseStream.Length);
            }
            else
                sendError(nws, "Comando invalido");

        }

        private void playerAction(NetworkStream nws, User user, string direction, ActionType actionType)
        {
            try
            {
                if (actionType == ActionType.MOVEMENT)
                {
                    if (Match.MovementCommands.ContainsKey(direction))
                    {
                        string closePlayers = Match.MovePlayer(user, Match.MovementCommands[direction]);
                        byte[] responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.MOVEMENT, "200|" + closePlayers);
                        nws.Write(responseStream, 0, responseStream.Length);
                    }
                    else
                        sendError(nws, "Comando invalido");
                }
                else if (actionType == ActionType.ATTACK)
                {
                    if (Match.MovementCommands.ContainsKey(direction))
                    {
                        string attackResponse = Match.PlayerAttack(user, Match.MovementCommands[direction]);
                        byte[] responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.ATTACK, "200|" + attackResponse);
                        nws.Write(responseStream, 0, responseStream.Length);
                    }
                    else
                        sendError(nws, "Comando invalido");
                }
            }
            catch (OccupiedSlotException)
            {
                sendError(nws, "Ya existe un usuario en esa posición");
            }
            catch (InvalidSurvivorAttackException)
            {
                sendError(nws, "Solo puede atacar monstruos");
            }
            catch (InvalidMoveException)
            {
                sendError(nws, "El movimiento que quiere realizar es inválido");
            }
            catch (MoveOutOfBoundsException)
            {
                sendError(nws, "El movimiento que quiere realizar esta fuera de los limites.");
            }
            catch (EndOfMatchException)
            {
                string winnersString = "";
                foreach (User winner in Match.Winners)
                {
                    winnersString += winner.NickName + ", ";
                }
                byte[] responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.END_OF_MATCH, ProtocolConstants.OK_RESPONSE_CODE);
                nws.Write(responseStream, 0, responseStream.Length);
            }
            catch (SurvivorsWinException)
            {
                string winnersString = "";
                foreach (User winner in Match.Winners)
                {
                    winnersString += winner.NickName + ", ";
                }
                byte[] responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.END_OF_MATCH, "300|" + winnersString);
                nws.Write(responseStream, 0, responseStream.Length);
            }
            catch (MonsterWinsException)
            {
                string winnersString = "";
                foreach (User winner in Match.Winners)
                {
                    winnersString += winner.NickName + ", ";
                }
                byte[] responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.END_OF_MATCH, "400|" + winnersString);
                nws.Write(responseStream, 0, responseStream.Length);
            }
            catch (UserTurnLimitException)
            {
                sendError(nws, "Debe esperar a que todos los jugadores realizan terminen su turno.");
            }
            catch (UserNotInMatchException)
            {
                byte[] responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.END_OF_MATCH, "500");
                nws.Write(responseStream, 0, responseStream.Length);
            }
            catch (Exception)
            {
                sendError(nws, "Ocurrió un error inesperado.");
            }
        }


        private void sendError(NetworkStream nws, string message)
        {
            byte[] errorData = Protocol.GenerateServerError(message);
            nws.Write(errorData, 0, errorData.Length);
        }

        private void joinMatch(User user, NetworkStream nws)
        {
            byte[] responseStream;
            if (Match.Active)
            {
                if (!Match.Users.Contains(user))
                {
                    Match.AddUserToMatch(user);
                }
                responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.JOIN_MATCH, ProtocolConstants.OK_RESPONSE_CODE);
                nws.Write(responseStream, 0, responseStream.Length);
            }
            else
            {
                sendError(nws, "No existe actualmente una partida activa.");
            }
        }

        private void sendAuthorizatonData(string data, NetworkStream nws, TcpClient client, ref User user)
        {
            byte[] responseStream;
            lock (registrationLock)
            {
                if (!RegisteredUsers.ContainsValue(user))
                {
                    RegisteredUsers.Add(client, user);
                    showRegisteredPlayers();
                    showConnectedPlayers();
                    responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.LOGIN, ProtocolConstants.OK_RESPONSE_CODE);
                }
                else
                {
                    User userInServer = RegisteredUsers.FirstOrDefault(x => x.Value.NickName.Equals(data)).Value;
                    TcpClient clientInServer = RegisteredUsers.FirstOrDefault(x => x.Value.NickName.Equals(data)).Key;
                    if (userInServer.Connected)
                    {
                        responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.LOGIN, "400");
                    }
                    else
                    {

                        RegisteredUsers.Remove(clientInServer);
                        RegisteredUsers.Add(client, user);
                        showRegisteredPlayers();
                        showConnectedPlayers();
                        responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.LOGIN, ProtocolConstants.OK_RESPONSE_CODE);
                    }
                }
                nws.Write(responseStream, 0, responseStream.Length);
            }
        }

        private void sendFileResponse(NetworkStream nws)
        {
            byte[] responseStream;
            responseStream = Protocol.GenerateStream(ProtocolConstants.SendType.RESPONSE, ProtocolConstants.AVATAR_UPLOAD, ProtocolConstants.OK_RESPONSE_CODE);
            nws.Write(responseStream, 0, responseStream.Length);
        }


        internal bool CanStartMatch()
        {
            return !Match.Active;
        }

        private static void downloadFile(string name, int dataLength, int total, TcpClient client)
        {
            byte[] totalFile = new byte[dataLength];
            int totalRead = 0;
            for (int i = 0; i <= total; i++)
            {
                int partSize = 0;
                if (i < total)
                {
                    partSize = ProtocolConstants.PART_SIZE;
                }
                else
                {
                    partSize = (int)dataLength - (total * ProtocolConstants.PART_SIZE);
                }

                byte[] partOfFile = Protocol.GetData(client, partSize);
                System.Buffer.BlockCopy(partOfFile, 0, totalFile, totalRead, partOfFile.Length);
                totalRead += partSize;
            }
            MemoryStream stream = new MemoryStream(totalFile);
            using (FileStream fileStream = File.Create(startupPath + "\\" + name, (int)stream.Length))
            {
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }
    }
}