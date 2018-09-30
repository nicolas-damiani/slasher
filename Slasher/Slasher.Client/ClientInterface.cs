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

        public ClientInterface()
        {
            clientLogic = new ClientLogic();
        }

        public void start()
        {
            mainMenu();
        }

        private void mainMenu()
        {
            int option = 0;
            while (clientLogic.Connected)
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
                        clientLogic.Connected = false;
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
                    Console.WriteLine(sendMovement());
                    break;
                case 2:
                    Console.WriteLine(attack());
                    break;
                case 3:
                    clientLogic.TcpClient.Close();
                    clientLogic.Connected = false;
                    clientLogic.InActiveMatch = false;
                    Console.WriteLine("Sesión desconectada.");
                    break;
                default:
                    Console.WriteLine("Comando invalido");
                    break;
            }
        }

        private string attack()
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
            try { 
            
                switch (option)
                {
                    case 1:
                        JoinToActiveMatch();
                        break;
                    case 2:
                        clientLogic.TcpClient.Close();
                        clientLogic.Connected = false;
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
                Console.WriteLine(clientLogic.JoinActiveMatch());
                clientLogic.InActiveMatch = true;
                while (clientLogic.InActiveMatch)
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
                    catch (EndOfMatchException)
                    {
                        Console.WriteLine("Partida Terminada");
                    }
                }
                showPreGameMenu();
            }
            catch (ClientException ex)
            {
                Console.WriteLine(ex.Message);
                showPreGameMenu();
            }
        }

        private void registerUser()
        {
            Console.WriteLine("REGISTRO DE USUARIO");
            Console.WriteLine("Ingrese un nombre de usuario");
            string nickname = Console.ReadLine();
            bool added = clientLogic.connect(nickname, "192.168.1.46", 6000);
            if (added)
            {
                Console.WriteLine("El usuario fue agregado exitosamente!");
                SendAvatar();
                SendCharacterType();
            }
            else
            {
                Console.WriteLine("El nombre de usuario ya esta utilizado, ingrese otro");
                registerUser();
            }
        }

        private void SendCharacterType()
        {
            Console.WriteLine("Seleccione un tipo de jugador ('sobreviviente' o 'monstruo')");
            string characterType = Console.ReadLine();
            try
            {
                Console.WriteLine(clientLogic.SendCharacterType(characterType));
            }
            catch (ClientException ex)
            {
                Console.WriteLine(ex.Message);
                SendAvatar();
            }
        }

        private void SendAvatar()
        {
            Console.WriteLine("Ingrese un ubicacion de imagen de avatar");
            string avatar = Console.ReadLine();
            try
            {
                Console.WriteLine(clientLogic.SendFile(avatar));
            }
            catch (ClientException ex)
            {
                Console.WriteLine(ex.Message);
                SendAvatar();
            }
        }

        private string sendMovement()
        {
            Console.WriteLine("Ingrese movimiento escribiendo el comando");
            string movement = Console.ReadLine();
            return clientLogic.SendMovement(movement);
        }
    }
}
