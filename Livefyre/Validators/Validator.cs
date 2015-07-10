using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Validators
{
    public interface Validator<T>
    {
            T Validate (T t);
    }
}
