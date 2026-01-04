using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<string> Errors { get; }
        public ValidationException(IEnumerable<string> errors) : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }
}
