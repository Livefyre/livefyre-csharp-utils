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

        // USE CONSTRUCTOR instead?
        public Site GetSite(String siteId, String siteKey) {
            return Site.Init(this, siteId, siteKey);
        }


        public Collection BuildBlogCollection(String title, String articleId, String url)
        {
            return BuildCollection(CollectionType.BLOG, title, articleId, url);
        }

        public Collection buildChatCollection(String title, String articleId, String url)
        {
            return BuildCollection(CollectionType.CHAT, title, articleId, url);
        }

        public Collection buildCommentsCollection(String title, String articleId, String url)
        {
            return BuildCollection(CollectionType.COMMENTS, title, articleId, url);
        }

        public Collection BuildCountingCollection(String title, String articleId, String url)
        {
            return BuildCollection(CollectionType.COUNTING, title, articleId, url);
        }

        public Collection BuildRatingsCollection(String title, String articleId, String url)
        {
            return BuildCollection(CollectionType.RATINGS, title, articleId, url);
        }

        public Collection BuildReviewsCollection(String title, String articleId, String url)
        {
            return BuildCollection(CollectionType.REVIEWS, title, articleId, url);
        }

        public Collection buildSidenotesCollection(String title, String articleId, String url)
        {
            return BuildCollection(CollectionType.SIDENOTES, title, articleId, url);
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
        public Collection BuildCollection(CollectionType type, String title, String articleId, String url)
        {
            return Collection.Init(this, type, title, articleId, url);
        }



    }
}
