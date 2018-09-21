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
        bool InActiveMatch { get; set; }

        public ClientInterface()
        {
            clientLogic = new ClientLogic();
            Connected = true;
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
            try
            {
                switch (option)
                {
                    case 1:
                        sendMovement();
                        break;
                    case 2:
                        attack();
                        break;
                    case 3:
                        clientLogic.TcpClient.Close();
                        Connected = false;
                        InActiveMatch = false;
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
            int option = 0;
            InActiveMatch = clientLogic.JoinActiveMatch();
            if (InActiveMatch)
            {
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
                }
                Console.WriteLine("Partida Terminada");
                showPreGameMenu();
            }
            else
            {
                Console.WriteLine("No se pudo unir a partida");
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
            InActiveMatch = !clientLogic.CheckGameStatus();
         }

        private void registerUser()
        {
            Console.WriteLine("REGISTRO DE USUARIO");
            Console.WriteLine("Ingrese un nombre de usuario");
            string nickname = Console.ReadLine();
            bool added = clientLogic.connect(nickname, "172.29.3.25", 6000);
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
            bool sendFileCorrectly = false;
            while (!sendFileCorrectly)
            {
                sendFileCorrectly = clientLogic.SendFile("nico.png");
                if (!sendFileCorrectly)
                    Console.Write("Ocurrió un error inesperado intente nuevamente");
                else
                    Console.Write("Imagen subida correctamente");
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
