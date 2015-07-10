using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Validators
{
    public interface Validator<T>
    {
//        string Validate<T>(T t);
        string Validate (T t);
    }
}
