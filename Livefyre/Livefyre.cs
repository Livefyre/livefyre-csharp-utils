using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Core;

namespace Livefyre
{
    public class Livefyre
    {
        /* Private constructor to prevent instantiation. */
        private Livefyre() { }

        public static Network GetNetwork(string networkName, string networkKey)
        {
            return Network.Init(networkName, networkKey);
        }
    }
}
