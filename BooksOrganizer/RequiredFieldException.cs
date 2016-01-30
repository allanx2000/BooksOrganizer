using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksOrganizer
{
    public class RequiredFieldException : Exception
    {
        private readonly string name;
        private readonly string message;

        public override string Message
        {
            get
            {
                return message;
            }
        }

        public RequiredFieldException(string fieldName) : base()
        {
            message = fieldName + " is required.";
        }
    }
}
