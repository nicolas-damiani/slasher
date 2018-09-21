using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Entities
{
    public class User
    {
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public Character Character { get; set; }
        public bool Connected { get; set; }

        public User(string nickname, string avatar)
        {
            NickName = nickname;
            Avatar = avatar;
            Connected = true;
        }

        public User()
        {
            Connected = true;
        }

        public override bool Equals(object objectToCompare)
        {
            if (objectToCompare == null || !objectToCompare.GetType().Equals(this.GetType()))
                return false;
            else
            {
                User compareUser = (User)objectToCompare;
                return this.NickName.Equals(compareUser.NickName);
            }
        }
    }
}
