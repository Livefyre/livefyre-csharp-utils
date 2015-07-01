using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Core
{
    class Collection
    {

        // PULLED FROM SITE.cs
        /* Default collection type */
        public Collection buildBlogCollection(String title, String articleId, String url)
        {
            return buildCollection(CollectionType.BLOG, title, articleId, url);
        }

        public Collection buildChatCollection(String title, String articleId, String url)
        {
            return buildCollection(CollectionType.CHAT, title, articleId, url);
        }

        public Collection buildCommentsCollection(String title, String articleId, String url)
        {
            return buildCollection(CollectionType.COMMENTS, title, articleId, url);
        }

        public Collection buildCountingCollection(String title, String articleId, String url)
        {
            return buildCollection(CollectionType.COUNTING, title, articleId, url);
        }

        public Collection buildRatingsCollection(String title, String articleId, String url)
        {
            return buildCollection(CollectionType.RATINGS, title, articleId, url);
        }

        public Collection buildReviewsCollection(String title, String articleId, String url)
        {
            return buildCollection(CollectionType.REVIEWS, title, articleId, url);
        }

        public Collection buildSidenotesCollection(String title, String articleId, String url)
        {
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
        public Collection buildCollection(CollectionType type, String title, String articleId, String url)
        {
            return Collection.init(this, type, title, articleId, url);
        }
    }
}
