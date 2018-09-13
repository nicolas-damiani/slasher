using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientInterface clientInterface = new ClientInterface();
            clientInterface.start();
        }
    }
}
