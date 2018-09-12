using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Entities
{
    class Match
    {
        public List<User> Users { get; set; }
        public User [,] Map { get; set; } 
        public bool Active { get; set; }


    }
}
