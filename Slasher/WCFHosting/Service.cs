using Slasher.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slasher.Entities;
using System.Net.Sockets;
using System.Runtime.Remoting;

namespace WCFHosting
{
    public class Service : IService
    {
        public ServerSystemController remoteObject;
        public Service() {
            remoteObject = (ServerSystemController)Activator.GetObject(
                            typeof(ServerSystemController),
                            "ServerSystemControllerUri",
                            WellKnownObjectMode.Singleton);
        }

        public void AddUserToSystem(string name)
        {
            remoteObject.AddUserToSystem(name);
        }

        public void DeleteUser(string username)
        {
            remoteObject.DeleteUser(username);
        }

        public List<User> GetRegisteredUsers()
        {
            return remoteObject.GetRegisteredUsers();
        }

        public void ModifyUser(string newName)
        {
            remoteObject.ModifyUser(newName);
        }
    }
}
