using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Model;

namespace Livefyre.Core
{
    class Site : LFCore
    {

        private Network network;
        private SiteData data;

        public Site(Network network, SiteData data) {
            this.network = network;
            this.data = data;
        }
            // BAKE INIT into Constructor?
        public static Site Init(Network network, String siteId, String siteKey) {
            SiteData data = new SiteData(siteId, siteKey);
            return new Site(network, ReflectiveValidator.validate(data));
        }

        /* Getters/Setters */
        public String GetUrn() {
            return network.GetUrn() + ":site=" + data.GetId();
        }
    
        public Network GetNetwork() {
            return network;
        }

        public void SetNetwork(Network network) {
            this.network = network;
        }

        public SiteData GetData() {
            return data;
        }

        public void SetData(SiteData data) {
            this.data = data;
        }

        // USE CONSTRUCTOR instead
        public Site GetSite(String siteId, String siteKey) {
            return Site.Init(this, siteId, siteKey);
        }
    }
}
