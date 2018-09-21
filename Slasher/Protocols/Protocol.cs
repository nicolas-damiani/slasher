using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocols
{
    public static class Protocol
    {

        public enum SendType { REQUEST, RESPONSE }

        public static byte[] getStreamAuthenticationUser(string name)
        {
            string dataSize = name.Length.ToString();
            while (dataSize.Length < 4)
            {
                dataSize = "0" + dataSize;
            }
            string stream = "REQ" + "01" + dataSize + name;
            return Encoding.ASCII.GetBytes(stream);
        }


        public static string makeSizeText(string size)
        {
            while (size.Length < 4)
            {
                size = "0" + size;
            }
            return size;
        }

        public static byte[] GetData(TcpClient tcpClient, int size)
        {
            int bufferSize = tcpClient.ReceiveBufferSize;
            NetworkStream dataStream = tcpClient.GetStream();
            byte[] data = new byte[size];
            int pos = 0;
            int currentData = 0;

            while (pos < size)
            {
                try
                {
                    currentData = dataStream.Read(data, pos, size - pos);
                    pos += currentData;
                    if (currentData == 0)
                    {
                        dataStream.Close();
                        tcpClient.Close();
                        data = null;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    tcpClient.Close();
                    return null;
                }
            }
            return data;
        }


        public static int getCommandAction(byte[] stream)
        {
            string resultString = System.Text.Encoding.ASCII.GetString(stream);
            string commandCodeString = resultString.Substring(3, 2);
            int commandCode = Int32.Parse(commandCodeString);
            return commandCode;
        }

        public static int GetDataLength(byte[] data)
        {
            string result = System.Text.Encoding.ASCII.GetString(data);
            string length = result.Substring(5, 4);
            int largoInt = Int32.Parse(length);
            return largoInt;
        }

        public static byte[] GenerateStream(SendType type, string command, string data)
        {
            string request = "";
            if (type == SendType.REQUEST)
            {
                request += "REQ";
            }
            else
            {
                request += "RES";
            }
            request += command + makeSizeText(data.Length.ToString()) + data;
            return stringToBytes(request);
        }

        private static byte[] stringToBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }


        public static bool checkIfLogged(byte[] response)
        {
            string responseText = Encoding.ASCII.GetString(response);
            string result = responseText.Substring(9, 3);
            if (result.Equals("200"))
            {
                Console.WriteLine("connected");
                return true;
            }
            else
            {
                Console.WriteLine("no connected");
                return false;
            }
        }
    }
}
