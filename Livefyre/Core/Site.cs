using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Core
{
    class Site : Core
    {

    private Network network;
    private SiteData data;

    public Site(Network network, SiteData data) {
        this.network = network;
        this.data = data;
    }
        // BAKE INIT into Constructor?
    public static Site init(Network network, String siteId, String siteKey) {
        SiteData data = new SiteData(siteId, siteKey);
        return new Site(network, ReflectiveValidator.validate(data));
    }


    /* Getters/Setters */
    public String getUrn() {
        return network.getUrn() + ":site=" + data.getId();
    }
    
    public Network getNetwork() {
        return network;
    }

    public void setNetwork(Network network) {
        this.network = network;
    }

    public SiteData getData() {
        return data;
    }

    public void setData(SiteData data) {
        this.data = data;
    }


        // MOVE TO SITE.cs
        /**
         * Constructs a new Site object based on the parameters passed in.
         * 
         * @param siteId the id for the Site.
         * @param siteKey the secret key for the Site.
         * @return Site
         */

        // USE CONSTRUCTOR instead
        public Site getSite(String siteId, String siteKey)
        {
            return Site.init(this, siteId, siteKey);
        }
    }
}
