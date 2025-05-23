using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.Route_Netwerk_BL.Exceptions
{
    public class RouteException : Exception
    {
        public RouteException()
        {
        }

        public RouteException(string? message) : base(message)
        {
        }

        public RouteException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
