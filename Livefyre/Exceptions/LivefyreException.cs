using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Exceptions
{

    public class LivefyreException : Exception
    {
        public LivefyreException()
        {
        }

        public LivefyreException(string message) : base(message)
        {
        }

        public LivefyreException(string message, Exception inner) : base(message, inner)
        {
        }
    }

}
