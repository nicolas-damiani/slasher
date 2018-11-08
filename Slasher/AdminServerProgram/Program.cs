using Exceptions;
using Slasher.Entities;
using Slasher.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace AdminServerProgram
{
    class Program
    {
        public static ServerSystemController remoteSystem;

        static void Main(string[] args)
        {

            IDictionary props = new Hashtable();
            props["port"] = 0;
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            new TcpChannel(props, null, serverProvider);
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            TcpChannel chan = new TcpChannel(props, null, serverProvider);
            ChannelServices.RegisterChannel(chan, false);

            remoteSystem = (ServerSystemController)Activator.GetObject(
                            typeof(ServerSystemController),
                            "tcp://localhost:1234/ServerSystemControllerUri");
            bool running = true;
            while (running)
            {
                try
                {
                    ShowMenu();
                    int opc = Int32.Parse(Console.ReadLine());
                    if (opc == 1)
                    {
                        AddUserToSystem(null);
                    }
                    if (opc == 2)
                    {

                    }
                    if (opc == 3)
                    {

                    }
                    if (opc == 4)
                    {
                        running = false;
                    }
                }
                catch (ServerSystemException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        public static void ShowMenu()
        {
            Console.WriteLine("1) Agregar usuario");
            Console.WriteLine("2) Modificar usuario");
            Console.WriteLine("3) Eliminar usuario");
            Console.WriteLine("4) Eliminar usuario");
        }


        public static void AddUserToSystem(User user)
        {
            Console.WriteLine("Ingrese nombre de usuario");
            remoteSystem.AddUserToSystem(Console.ReadLine());
            Console.WriteLine("Usuario ingresado correctamente.");
        }

        public static void DeleteUserInSystem(string username)
        {
            Console.WriteLine("Ingrese nombre de usuario");
            remoteSystem.DeleteUser(Console.ReadLine());
            Console.WriteLine("Usuario eliminado correctamente.");
        }

        public static void ModifyUserInSystem(User user)
        {
            Console.WriteLine("Ingrese nombre de usuario");
            remoteSystem.ModifyUser(Console.ReadLine());
            Console.WriteLine("Usuario modificado correctamente.");
        }
    }
}
