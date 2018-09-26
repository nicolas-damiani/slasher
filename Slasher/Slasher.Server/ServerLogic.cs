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

namespace Slasher.Server
{
    class ServerLogic
    {
        public Match Match { get; set; }
        private List<TcpClient> TcpClients { get; set; }
        private Dictionary<TcpClient, User> RegisteredUsers { get; set; }
        public bool Connected { get; set; }
        private static string startupPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Archivos";
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
                entry.Key.Close();
                entry.Value.Connected = false;
            }
            Connected = false;
        }

        internal List<User> GetPossibleUsersToAdd()
        {
            throw new NotImplementedException();
        }

        internal void AcceptUser(User user)
        {
            throw new NotImplementedException();
        }

        internal void ConnectServer()
        {
            IPAddress ipAddress = IPAddress.Parse("192.168.1.125");
            Listener = new TcpListener(ipAddress, 6000);
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
                catch (SocketException ex) { Console.WriteLine(ex.Message); }
            }
        }

        private void receiveCommands(TcpClient client)
        {
            User user = new User();
            while (user.Connected)
            {
                byte[] headerInformation = new byte[Protocol.HEADER_SIZE + 5];
                headerInformation = Protocol.GetData(client, Protocol.HEADER_SIZE + 5);
                if (headerInformation != null)
                    executeCommand(headerInformation, client, ref user);
                else
                {
                    user.Connected = false;
                }
            }
        }

        private void executeCommand(byte[] headerInformation, TcpClient client, ref User user)
        {
            NetworkStream nws = client.GetStream();
            int dataLength = Protocol.GetDataLength(headerInformation);
            byte[] data = new byte[dataLength];
            data = Protocol.GetData(client, dataLength);
            int command = Protocol.getCommandAction(headerInformation);

            switch (command)
            {
                case 01:
                    string name = UnicodeEncoding.ASCII.GetString(data);
                    user.NickName = name;
                    sendAuthorizatonData(name, nws, client, ref user);
                    break;
                case 10:
                    downloadFile("nico.png", data);
                    sendFileResponse(nws);
                    break;
                case 20:
                    joinMatch(RegisteredUsers[client], nws);
                    break;
                case 30:
                    respondWhenMatchFinishes(nws);
                    break;
                case 40:
                    string movement = UnicodeEncoding.ASCII.GetString(data);
                    movePlayer(nws, RegisteredUsers[client], movement);
                    break;
            };
        }

        private void movePlayer(NetworkStream nws, User user, string direction)
        {
            try
            {
                Match.MovePlayer(user, Match.MovementCommands[direction]);
                byte[] responseStream = Protocol.GenerateStream(Protocol.SendType.RESPONSE, "41", "200");
                nws.Write(responseStream, 0, responseStream.Length);
            }
            catch (OccupiedSlotException)
            {
                sendError(nws, "Ya existe un usuario en esa posición");
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
                byte[] responseStream = Protocol.GenerateStream(Protocol.SendType.RESPONSE, "61", "200");
                nws.Write(responseStream, 0, responseStream.Length);
            }
            catch (Exception)
            {
                sendError(nws, "Ocurrió un error inesperado.");
            }
        }

        private void respondWhenMatchFinishes(NetworkStream nws)
        {
            Thread matchFinishesThread = new Thread(() => checkMatchFinalizes(nws));
            matchFinishesThread.Start();
        }

        private void checkMatchFinalizes(NetworkStream nws)
        {
            while (Match.Active) { };
            byte[] responseStream = Protocol.GenerateStream(Protocol.SendType.RESPONSE, "31", "200");
            nws.Write(responseStream, 0, responseStream.Length);
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
                responseStream = Protocol.GenerateStream(Protocol.SendType.RESPONSE, "21", "200");
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
                    responseStream = Protocol.GenerateStream(Protocol.SendType.RESPONSE, "01", "200");
                }
                else
                {
                    responseStream = Protocol.GenerateStream(Protocol.SendType.RESPONSE, "01", "400");
                }
            }
            nws.Write(responseStream, 0, responseStream.Length);
        }

        private void sendFileResponse(NetworkStream nws)
        {
            byte[] responseStream;
            responseStream = Protocol.GenerateStream(Protocol.SendType.RESPONSE, "11", "200");
            nws.Write(responseStream, 0, responseStream.Length);
        }

        internal List<User> GetRegisteredUsers()
        {
            throw new NotImplementedException();
        }

        internal List<User> GetConnectedPlayers()
        {
            throw new NotImplementedException();
        }

        internal bool CanStartMatch()
        {
            return !Match.Active;
        }

        public static void SaveBytesToFile(string filename, byte[] bytesToWrite)
        {
            filename = "C:\\Users\\Usuario\\Desktop";
            if (filename != null && filename.Length > 0 && bytesToWrite != null)
            {
                if (!Directory.Exists(Path.GetDirectoryName(filename)))
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));

                FileStream file = File.Create(filename);

                file.Write(bytesToWrite, 0, bytesToWrite.Length);

                file.Close();
            }
        }

        private static void downloadFile(string name, byte[] fileData)
        {
            name = "nico.png";
            /*  if (File.Exists(startupPath + "\\" + name))
              {
                  File.Delete(startupPath + "\\" + name);
              }
              FileStream fs = File.Create(startupPath + "\\" + name);
              fs.Write(fileData, 0, fileData.Length);
              fs.Close();
              */
            MemoryStream stream = new MemoryStream(fileData);
            using (FileStream fileStream = File.Create(startupPath + "\\" + name, (int)stream.Length))
            {
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }
    }
}