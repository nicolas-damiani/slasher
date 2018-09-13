using Slasher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            while (true)
            {
                try
                {
                    showMainMenu();
                    option = int.Parse(Console.ReadLine());
                    menuSwitch(option);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Comando invalido");
                }
            }
        }

        private void showMainMenu()
        {
            Console.WriteLine("*******************************");
            Console.WriteLine("1) REGISTRAR USUARIO");
            Console.WriteLine("2) CONECTARSE AL SERVIDOR");
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
                        break;
                    default:
                        Console.WriteLine("Comando invalido");
                        break;
                }
            }
            //PONER EXCEPTION NUESTRA
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void registerUser()
        {
            Console.WriteLine("REGISTRO DE USUARIO");
            Console.WriteLine("Ingrese un nombre de usuario");
            string nickname = Console.ReadLine();
            Console.WriteLine("Ingrese un avatar (nombre)");
            string avatar = Console.ReadLine();
            bool added = clientLogic.createUser(nickname, avatar);
            if (added)
            {
                Console.WriteLine("El usuario fue agregado exitosamente!");
            }
            else
            {
                Console.WriteLine("El nombre de usuario ya esta utilizado, ingrese otro");
                registerUser();
            }

        }


    }
}
