using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngulareQueues.Model.Exceptions
{
    public class OfficeHoursException : Exception
    {
        public OfficeHoursException() : base() { }
        public OfficeHoursException(string message) : base(message) { }
    }
}
