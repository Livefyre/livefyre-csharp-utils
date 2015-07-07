using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;


using Livefyre.Core;
using Livefyre.Utils;

namespace Livefyre.Api.Filter
{
 
    public class LftokenAuthFilter  {

        private LFCore core;
        private string userToken;
    

        public LftokenAuthFilter(LFCore core, String userToken) {
            this.core = core;
            this.userToken = userToken;
        }
    
        // not nec here?  overriding behavior in Java lib
        public WebClient handle(WebClient wc) {
            // check these methods for throw sig
            string lftoken = (userToken == null ? LivefyreUtil.GetNetworkFromCore(core).BuildLivefyreToken() : userToken);

            wc.Headers.Add("Authorization", lftoken);

            return wc;
        }
}


}
