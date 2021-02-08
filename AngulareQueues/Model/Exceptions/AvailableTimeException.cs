using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngulareQueues.Model.Exceptions
{
    public class AvailableTimeException : Exception
    {
        public AvailableTimeException() : base() { }
        public AvailableTimeException(string message) : base(message) { }
    }
}
