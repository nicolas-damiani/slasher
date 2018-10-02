using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocols
{
    public static class Protocol
    {
        public static string makeSizeHeader(string size)
        {
            while (size.Length < ProtocolConstants.DATA_HEADER_SIZE)
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
                        size = 0;
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

        public static int getTotalPartsFromFile(byte[] stream)
        {
            string resultString = System.Text.Encoding.ASCII.GetString(stream);
            string commandCodeString = resultString.Substring(0, 2);
            int commandCode = Int32.Parse(commandCodeString);
            return commandCode;
        }

        public static int GetDataLength(byte[] data)
        {
            string result = System.Text.Encoding.ASCII.GetString(data);
            string length = result.Substring(5, ProtocolConstants.DATA_HEADER_SIZE);
            int largoInt = Int32.Parse(length);
            return largoInt;
        }

        public static byte[] GenerateStream(ProtocolConstants.SendType type, int command, string data)
        {
            string request = "";
            if (type == ProtocolConstants.SendType.REQUEST)
            {
                request += "REQ";
            }
            else
            {
                request += "RES";
            }
            request += makeSizeTwo(command.ToString()) + makeSizeHeader(data.Length.ToString()) + data;
            return stringToBytes(request);
        }

        private static byte[] stringToBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }


        public static bool checkIfLogged(byte[] response)
        {
            string responseText = Encoding.ASCII.GetString(response);
            string result = responseText.Substring(ProtocolConstants.HEADER_SIZE, 3);
            if (result.Equals(ProtocolConstants.OK_RESPONSE_CODE))
            {
                return true;
            }
            else
            {
                Console.WriteLine("no connected");
                return false;
            }
        }

        public static byte[] GenerateServerError(string message)
        {
            string response;
            response = "RES" + makeSizeTwo(ProtocolConstants.ERROR.ToString()) + makeSizeHeader(message.Length + "") + message;
            return stringToBytes(response);
        }

        public static byte[] ReadFully(FileStream sourceFile, int i, int parts, ref int totalRead)
        {
            int read = 0;
            int partSize = 0;
            if (i < parts)
            {
                partSize = ProtocolConstants.PART_SIZE;
            }
            else
            {
                partSize =  (int)sourceFile.Length - ( i* ProtocolConstants.PART_SIZE) ; 
            }
            byte[] total;
            byte[] output = new byte[partSize];
            while (read < partSize)
            {
                read = sourceFile.Read(output, read, partSize - read);
            }
            totalRead += read;
            if (i == 0)
            {
                string response;
                response = "REQ" + makeSizeTwo(ProtocolConstants.AVATAR_UPLOAD.ToString()) + makeSizeHeader(sourceFile.Length + "") + makeSizeTwo((parts).ToString());
                byte[] requestBytes = stringToBytes(response);
                total = new byte[requestBytes.Length + output.Length];
                System.Buffer.BlockCopy(requestBytes, 0, total, 0, requestBytes.Length);
                System.Buffer.BlockCopy(output, 0, total, requestBytes.Length, output.Length);
                return total;
            }
            else
            {
                return output;
            }

        }

        public static string makeSizeTwo(string size)
        {
            while (size.Length < 2)
            {
                size = "0" + size;
            }
            return size;
        }
    }
}
