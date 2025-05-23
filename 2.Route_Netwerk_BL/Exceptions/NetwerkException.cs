using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.Route_Netwerk_BL.Exceptions
{
    public class NetwerkException : Exception
    {
        public NetwerkException()
        {
        }

        public NetwerkException(string? message) : base(message)
        {
        }

        public NetwerkException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
