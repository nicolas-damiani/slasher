using Exceptions;
using Slasher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Slasher.Client
{
    public class ClientInterface
    {
        private ClientLogic clientLogic;
        private bool Connected { get; set; }
        private bool InActiveMatch { get; set; }
        private string finalizedMatchError;

        public ClientInterface()
        {
            clientLogic = new ClientLogic();
            Connected = true;
            InActiveMatch = false;
            finalizedMatchError = "";
        }

        public void start()
        {
            mainMenu();
        }

        private void mainMenu()
        {
            int option = 0;
            while (Connected)
            {
                try
                {
                    showInitialMenu();
                    option = int.Parse(Console.ReadLine());
                    menuSwitch(option);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Comando invalido");
                }
            }
            Console.WriteLine("JUGADOR DESCONECTADO");
            Console.WriteLine();
        }

        private void showInitialMenu()
        {
            Console.WriteLine("*******************************");
            Console.WriteLine("1) REGISTRAR USUARIO");
            Console.WriteLine("2) DESCONECTARSE");
            Console.WriteLine("*******************************\n");
        }

        private void showGameMenu()
        {
            Console.WriteLine("*******************************");
            Console.WriteLine("1) ATACAR");
            Console.WriteLine("2) MOVER");
            Console.WriteLine("3) DESCONECTARSE");
            Console.WriteLine("*******************************\n");
        }

        private void menuSwitch(int option)
        {
            try
            {
                switch (option)
                {
                    case 1:
                        registerUser();
                        showPreGameMenu();
                        break;
                    case 2:
                        clientLogic.TcpClient.Close();
                        Connected = false;
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


        private void menuGameSwitch(int option)
        {
            switch (option)
            {
                case 1:
                    sendMovement();
                    Console.WriteLine("Movimiento realizado correctamente");
                    break;
                case 2:
                    attack();
                    Console.WriteLine("Ataque realizado correctamente");
                    break;
                case 3:
                    clientLogic.TcpClient.Close();
                    Connected = false;
                    InActiveMatch = false;
                    Console.WriteLine("Sesión desconectada.");
                    break;
                default:
                    Console.WriteLine("Comando invalido");
                    break;
            }
        }

        private void attack()
        {
            throw new NotImplementedException();
        }

        private void showPreGameMenu()
        {
            Console.WriteLine("*******************************");
            Console.WriteLine("1) UNIRSE A PARTIDA ACTIVA");
            Console.WriteLine("2) DESCONECTARSE");
            Console.WriteLine("*******************************");
            int option = int.Parse(Console.ReadLine());
            try
            {
                switch (option)
                {
                    case 1:
                        JoinToActiveMatch();
                        break;
                    case 2:
                        clientLogic.TcpClient.Close();
                        Connected = false;
                        break;
                    default:
                        Console.WriteLine("Comando invalido");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                showPreGameMenu();
            }

        }

        private void JoinToActiveMatch()
        {
            try
            {
                int option = 0;
                clientLogic.JoinActiveMatch();
                InActiveMatch = true;
                Thread threadInActiveMatch = new Thread(IsInActiveMatch);
                threadInActiveMatch.Start();
                while (InActiveMatch)
                {
                    try
                    {
                        showGameMenu();
                        option = int.Parse(Console.ReadLine());
                        menuGameSwitch(option);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Comando inválido");
                    }
                    catch (ClientException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine("Partida Terminada");
                showPreGameMenu();
            }
            catch (ClientException ex)
            {
                Console.WriteLine(ex.Message);
                showPreGameMenu();
            }
        }

        internal void IsInActiveMatch()
        {
            /* bool inActiveMatchThread = true;
             while (inActiveMatchThread)
             {

                 if (!clientLogic.CheckGameStatus())
                 {
                     inActiveMatchThread = clientLogic.CheckGameStatus();
                     Console.WriteLine("Partida finalizada");
                 }
                 inActiveMatchThread = clientLogic.CheckGameStatus() && InActiveMatch;
             }
             InActiveMatch = false;*/
         /*   try
            {
                finalizedMatchError = "";
                clientLogic.CheckGameStatus();
                InActiveMatch = false;
            }
            catch (ClientException ex)
            {
                finalizedMatchError = ex.Message;
                InActiveMatch = false;
            }*/
        }

        private void registerUser()
        {
            Console.WriteLine("REGISTRO DE USUARIO");
            Console.WriteLine("Ingrese un nombre de usuario");
            string nickname = Console.ReadLine();
            bool added = clientLogic.connect(nickname, "192.168.1.125", 6000);
            if (added)
            {
                Console.WriteLine("El usuario fue agregado exitosamente!");
                SendAvatar();
            }
            else
            {
                Console.WriteLine("El nombre de usuario ya esta utilizado, ingrese otro");
                registerUser();
            }
        }

        private void SendAvatar()
        {
            Console.WriteLine("Ingrese un ubicacion de imagen de avatar");
            string avatar = Console.ReadLine();
            try
            {
                clientLogic.SendFile("nico.png");
                Console.WriteLine("Imagen subida correctamente.");
            }
            catch (ClientException ex)
            {
                Console.WriteLine(ex.Message);
                SendAvatar();
            }
        }

        private void sendMovement()
        {
            Console.WriteLine("Ingrese movimiento escribiendo el comando");
            string movement = Console.ReadLine();
            clientLogic.SendMovement(movement);
        }


    }
}
