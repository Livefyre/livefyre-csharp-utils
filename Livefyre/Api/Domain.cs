using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Livefyre.Core;
using Livefyre.Utils;

namespace Livefyre.Api
{

    public class Domain
    {
        public static String quill(Core core)
        {
            Network network = GetNetworkFromCore(core);
            return network.isSsl() ? String.format("https://%s.quill.fyre.co",
                    network.getNetworkName()) : String.format("http://quill.%s.fyre.co", network.getNetworkName());
        }

        public static String bootstrap(Core core)
        {
            Network network = GetNetworkFromCore(core);
            return network.isSsl() ? String.format("https://%s.bootstrap.fyre.co",
                    network.getNetworkName()) : String.format("http://bootstrap.%s.fyre.co", network.getNetworkName());
        }
    }
}
