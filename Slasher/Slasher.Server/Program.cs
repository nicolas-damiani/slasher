﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Slasher.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerInterface serverInterface = new ServerInterface();
            serverInterface.Start();
        }
    }
}
