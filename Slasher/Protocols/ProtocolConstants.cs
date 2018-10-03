using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocols
{
    public static class ProtocolConstants
    {


        public enum SendType { REQUEST, RESPONSE }
        public static int DATA_HEADER_SIZE = 8;
        public static int HEADER_SIZE = 5 + DATA_HEADER_SIZE;
        public static int PART_SIZE = 8192;
        public static string OK_RESPONSE_CODE = "200";


        public const int LOGIN = 01;
        public const int AVATAR_UPLOAD = 10;
        public const int SELECT_CHARACTER = 15;
        public const int JOIN_MATCH = 20;
        public const int MOVEMENT = 40;
        public const int ATTACK = 50;
        public const int END_OF_MATCH = 60;
        public const int ERROR = 99;

    }
}
