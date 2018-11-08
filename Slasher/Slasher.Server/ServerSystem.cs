using Slasher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Server
{
    [Serializable]
    public class ServerSystem
    {
        public Dictionary<TcpClient, User> RegisteredUsers { get; set; }

        private ServerSystem()
        {
            RegisteredUsers = new Dictionary<TcpClient, User>();
        }

        public ServerSystem GetInstance()
        {
            if (RegisteredUsers == null)
            {
                return new ServerSystem();
            }
            else
            {
                return this;
            }
        }
    }
}
