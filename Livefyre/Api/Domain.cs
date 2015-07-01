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
        public static String quill(LFCore core)
        {
            Network network = LivefyreUtil.GetNetworkFromCore(core);
            return network.IsSsl() ? String.Format("https://{0}.quill.fyre.co",
                    network.GetNetworkName()) : String.Format("http://quill.{0}.fyre.co", network.GetNetworkName());
        }

        public static String bootstrap(LFCore core)
        {
            Network network = LivefyreUtil.GetNetworkFromCore(core);
            return network.IsSsl() ? String.Format("https://%s.bootstrap.fyre.co",
                    network.GetNetworkName()) : String.Format("http://bootstrap.%s.fyre.co", network.GetNetworkName());
        }
    }
}
