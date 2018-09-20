using Slasher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Slasher.Server
{
    public class ServerInterface
    {
        private ServerLogic ServerLogic { get; set; }


        public ServerInterface()
        {
            ServerLogic = new ServerLogic();
        }

        public void Start()
        {
            Console.WriteLine("Bienvenido al sistema Slasher Servidor!");

            Thread acceptConnections = new Thread(() => ServerLogic.ConnectServer());
            string exitCommand = Console.ReadLine();
            ServerLogic.Connected = false;
        }



        private void ShowMainMenu()
        {
            int option = 0;
            while (true)
            {
                try
                {
                    showOptions();
                    option = int.Parse(Console.ReadLine());
                    menuSwitch(option);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Comando invalido");
                }
            }
        }

        private void showOptions()
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine("1) ACEPTAR PEDIDOS DE CONEXIÓN");
            Console.WriteLine("2) DAR DE ALTA UN JUGADOR");
            Console.WriteLine("3) MOSTRAR JUGADORES REGISTRADOS");
            Console.WriteLine("4) MOSTRAR JUGADORES CONECTADOS");
            Console.WriteLine("5) INICIAR PARTIDA");
            Console.WriteLine("6) ACEPTAR JUGADORES A PARTIDA");
            Console.WriteLine("-------------------------------");
        }


        private void menuSwitch(int option)
        {
            try
            {
                switch (option)
                {
                    case 1:
                        break;
                    case 2:
                        {
                            List<User> users = ServerLogic.GetPossibleUsersToAdd();
                            if (users != null)
                            {
                                Console.WriteLine("Elija jugador para dar de alta");
                                for (int i = 0; i < users.Count; i++)
                                {
                                    Console.WriteLine(i + ") " + users[i].NickName);
                                }

                                int optionUser = 0;
                                optionUser = int.Parse(Console.ReadLine());
                                ServerLogic.AcceptUser(users[optionUser]);
                            }
                            else
                            {
                                Console.WriteLine("No hay usuarios para dar de alta");
                            }
                            break;
                        }
                    case 3:
                        {
                            List<User> users = ServerLogic.GetRegisteredUsers();
                            if (users != null)
                            {
                                Console.WriteLine("Jugadores Registrados");
                                for (int i = 0; i < users.Count; i++)
                                {
                                    Console.WriteLine(i + ") " + users[i].NickName);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay usuarios registrados");
                            }
                        }
                        break;
                    case 4:
                        {
                            List<User> users = ServerLogic.GetConnectedPlayers();
                            if (users != null)
                            {
                                Console.WriteLine("Jugadores Conectados");
                                for (int i = 0; i < users.Count; i++)
                                {
                                    Console.WriteLine(i + ") " + users[i].NickName);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay usuarios conectados");
                            }
                        }
                        break;
                    case 5:
                        if (ServerLogic.CanStartMatch())
                        {
                            ServerLogic.StartMatch();
                        }
                        else
                        {
                            Console.WriteLine("Ya existe una partida activa");
                        }
                        break;
                    case 6:
                        break;
                    default:
                        Console.WriteLine("Comando invalido");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
