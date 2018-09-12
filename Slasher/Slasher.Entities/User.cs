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
    }
}
