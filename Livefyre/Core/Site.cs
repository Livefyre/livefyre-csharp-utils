using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Core
{
    class Site : LfCore
    {

    private Network network;
    private SiteData data;

    public Site(Network network, SiteData data) {
        this.network = network;
        this.data = data;
    }

    public static Site init(Network network, String siteId, String siteKey) {
        SiteData data = new SiteData(siteId, siteKey);
        return new Site(network, ReflectiveValidator.validate(data));
    }


    // MOVE TO OWN CLASS
    /* Default collection type */
    public Collection buildBlogCollection(String title, String articleId, String url) {
        return buildCollection(CollectionType.BLOG, title, articleId, url);
    }

    public Collection buildChatCollection(String title, String articleId, String url) {
        return buildCollection(CollectionType.CHAT, title, articleId, url);
    }

    public Collection buildCommentsCollection(String title, String articleId, String url) {
        return buildCollection(CollectionType.COMMENTS, title, articleId, url);
    }
    
    public Collection buildCountingCollection(String title, String articleId, String url) {
        return buildCollection(CollectionType.COUNTING, title, articleId, url);
    }
    
    public Collection buildRatingsCollection(String title, String articleId, String url) {
        return buildCollection(CollectionType.RATINGS, title, articleId, url);
    }
    
    public Collection buildReviewsCollection(String title, String articleId, String url) {
        return buildCollection(CollectionType.REVIEWS, title, articleId, url);
    }
    
    public Collection buildSidenotesCollection(String title, String articleId, String url) {
        return buildCollection(CollectionType.SIDENOTES, title, articleId, url);
    }
    
    /**
     * Creates and returns a Collection object. Be sure to call createOrUpdate() on it to inform Livefyre to
     * complete creation and any updates.
     * 
     * Options accepts a map of key/value pairs for your Collection. Some examples are 'tags',
     * 'type', 'extensions', 'tags', etc. Please refer to
     * http://answers.livefyre.com/developers/getting-started/tokens/collectionmeta/ for more info.
     *
     * @param type the type of the collection.
     * @param articleId the articleId for the collection.
     * @param title title for the collection.
     * @param url url for the collection.
     * 
     * @return Collection
     */
    public Collection buildCollection(CollectionType type, String title, String articleId, String url) {
        return Collection.init(this, type, title, articleId, url);
    }
    
    //build different collection types here

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
        public Site getSite(String siteId, String siteKey)
        {
            return Site.init(this, siteId, siteKey);
        }

        public String getUrn()
        {
            return "urn:livefyre:" + data.getName();
        }

        public String getUrnForUser(String user)
        {
            return getUrn() + ":user=" + user;
        }

        public String getNetworkName()
        {
            return data.getName().split("\\.")[0];
        }

        public Boolean isSsl()
        {
            return ssl;
        }

        public void setSsl(Boolean ssl)
        {
            this.ssl = ssl;
        }

        public NetworkData getData()
        {
            return data;
        }

        public void setData(NetworkData data)
        {
            this.data = data;
        }
    }
}
