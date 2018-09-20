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

namespace Slasher.Server
{
    class ServerLogic
    {
        public Match Match { get; set; }
        private List<TcpClient> TcpClients { get; set; }
        private Dictionary<TcpClient, User> RegisteredUsers { get; set; }
        public bool Connected { get; set; }
        private TcpListener Listener;


        public ServerLogic()
        {
            TcpClients = new List<TcpClient>();
            Match = new Match();
            RegisteredUsers = new Dictionary<TcpClient, User>();
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
            IPAddress ipAddress = IPAddress.Parse("192.168.1.53");
            Listener = new TcpListener(ipAddress, 6000);
            Console.WriteLine("local ip address: " + ipAddress);
            Listener.Start();
            while ( Connected )
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
                byte[] headerInformation = new byte[9];
                headerInformation = Protocol.GetData(client, 9);
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
                    sendAuthorizatonData(name, nws,client, ref user);
                    break;
                case 10:
                    string movement = UnicodeEncoding.ASCII.GetString(data);
                    Match.MovePlayer(RegisteredUsers[client], Match.MovementCommands[movement]);
                    break;
            };
        }

        private void sendAuthorizatonData(string data, NetworkStream nws, TcpClient client, ref User user)
        {
            byte[] responseStream;
            if (!RegisteredUsers.ContainsValue(user))
            {
                RegisteredUsers.Add(client, user);
                responseStream = Protocol.GenerateStream(Protocol.SendType.RESPONSE, "01", "200");
            }
            else
            {
                responseStream = Protocol.GenerateStream(Protocol.SendType.RESPONSE, "01", "400");
            }
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

        internal void StartMatch()
        {
            Match = new Match();
        //    Match.StartMatch();
        }
    }
}
