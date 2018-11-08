using Exceptions;
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
    public class ServerSystemController : MarshalByRefObject
    {
        private ServerSystem system;

        public ServerSystemController()
        {
        }

        public void AddUserToSystem(string name)
        {
            User user = new User(name);
            var registeredUsers = system.GetInstance().RegisteredUsers;
            if (!registeredUsers.ContainsValue(user))
            {
                registeredUsers.Add(new TcpClient(), user);
            }
            else
                throw new ServerSystemException("Ya existe un usuario con ese nombre.");
        }

        public Dictionary<TcpClient, User> GetRegisteredUsers()
        {
            return system.GetInstance().RegisteredUsers;
        }

        public void DeleteUser(string username)
        {
            User user = new User(username);
            var registeredUsers = system.GetInstance().RegisteredUsers;
            if (registeredUsers.ContainsValue(user))
            {
                User userInServer = registeredUsers.FirstOrDefault(x => x.Value.NickName.Equals(username)).Value;
                var item = registeredUsers.First(kvp => kvp.Value.Equals(userInServer));
                registeredUsers.Remove(item.Key);
            }
            else
                throw new ServerSystemException("No existe un usuario con ese nombre");

        }

        public void ModifyUser(string newName)
        {
            User user = new User(newName);
            var registeredUsers = system.GetInstance().RegisteredUsers;
            if (registeredUsers.ContainsValue(user))
            {
                User userInServer = registeredUsers.FirstOrDefault(x => x.Value.NickName.Equals(newName)).Value;
                var item = registeredUsers.First(kvp => kvp.Value.Equals(userInServer));
                userInServer.NickName = newName;
                registeredUsers[item.Key] = userInServer;
            }
        }
    }
}
