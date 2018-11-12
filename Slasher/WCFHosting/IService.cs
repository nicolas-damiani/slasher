using Slasher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFHosting
{
    [ServiceContract]
    public interface IService
    {

        [OperationContract]
        void AddUserToSystem(string name);

        [OperationContract]
        List<User> GetRegisteredUsers();

        [OperationContract]
        void DeleteUser(string username);

        [OperationContract]
        void ModifyUser(string newName);
    
    }
}
