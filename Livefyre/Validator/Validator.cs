using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Validator
{
    interface Validator
    {
        string Validate<T>(T type);
    }
}
