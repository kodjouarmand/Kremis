using System;
using System.Collections.Generic;
using System.Text;

namespace Kremis.BusinessLogic.Exceptions
{
    public class BllValidationException : Exception
    {
        public BllValidationException(string errorMessage): base(errorMessage)
        {
        }
    }
}
