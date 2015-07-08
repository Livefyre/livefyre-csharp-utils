using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Validator
{
    public interface Validator
    {
        string Validate(System.Type t);
    }
}
