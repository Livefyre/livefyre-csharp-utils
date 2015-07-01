using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Utils
{
    public class Precondition
    {

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <param name="reference">an object reference</param>
        /// <returns>the non-null reference that was validated</returns>
        /// <exception cref="NullReferenceException">if reference is null</exception>
        public static T CheckNotNull<T>(T reference)
        {
            if (reference == null)
            {
                throw new NullReferenceException();
            }

            return reference;
        }

        /// <summary>
        /// Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        /// <param name="reference">an object reference</param>
        /// <param name="errorMessage">the exception message to use if the check fails</param>
        /// <returns>the non-null reference that was validated</returns>
        /// <exception cref="NullReferenceException">if reference is null</exception>
        public static T CheckNotNull<T>(T reference, string errorMessage)
        {
            if (reference == null)
            {
                throw new NullReferenceException(errorMessage);
            }

            return reference;
        }
    }
}
