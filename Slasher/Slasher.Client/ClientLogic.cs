using Slasher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Client
{
    public class ClientLogic
    {
        private List<User> userList;

        public ClientLogic()
        {
            userList = new List<User>();
        }

        public bool createUser(string nickname, string avatar)
        {
            User user = new User(nickname, avatar);
            if (!userList.Contains(user))
            {
                userList.Add(user);
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
