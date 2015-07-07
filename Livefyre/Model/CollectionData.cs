using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Livefyre.Type;
using Livefyre.Dto;


namespace Livefyre.Model
{
    public class CollectionData
    {
        
        private CollectionType type;
        private string id;
        private string articleId;
        private string title;
        private string url;
    
        //optional params
        private string tags;
        private List<Topic> topics;
        private string extensions;
    
        public CollectionData(CollectionType type, string title, string articleId, string url) {
            this.type = type;
            this.articleId = articleId;
            this.title = title;
            this.url = url;
        }
        /*
        // make C# Dictionary
        public Map<string, Object> AsMap() {

            Map<string, Object> attr = Maps.newTreeMap();
            attr.put("articleId", articleId);
            attr.put("title", title);


            // work out the enum lang conversion puzzle
            attr.put("type", (string)type);
            attr.put("url", url);
        
            if (string.IsNullOrEmpty(tags)) {
                attr.put("tags", tags);
            }
            if (topics != null && topics.Length > 0) {
                attr.put("topics", topics);
            }
            if (string.IsNullOrEmpty(extensions))
            {
                attr.put("extensions", extensions);
            }
            return attr;
        }
        */
        // make C# set/get?
        public CollectionType GetType() {
            return type;
        }
    
        public CollectionData SetType(CollectionType type) {
            this.type = type;
            return this;
        }
    
        public string GetId() {
            if (id == null) {
                // make msg var/mem
                throw new Exception("Id not set. Call createOrUpdate() on the collection to set the id, or manually set it by calling setId(id) on this object.");
                // throw new LivefyreException("Id not set. Call createOrUpdate() on the collection to set the id, or manually set it by calling setId(id) on this object.");
            }
            return id;
        }
    
        public CollectionData SetId(string id) {
            this.id = id;
            return this;
        }

        public string GetArticleId() {
            return articleId;
        }

        public CollectionData SetArticleId(string articleId) {
            this.articleId = articleId;
            return this;
        }

        public string GetTitle() {
            return title;
        }

        public CollectionData SetTitle(string title) {
            this.title = title;
            return this;
        }

        public string GetUrl() {
            return url;
        }

        public CollectionData SetUrl(string url) {
            this.url = url;
            return this;
        }

        public string GetTags() {
            return tags;
        }

        public CollectionData SetTags(string tags) {
            this.tags = tags;
            return this;
        }

        public List<Topic> GetTopics() {
            return topics;
        }

        public CollectionData SetTopics(List<Topic> topics) {
            this.topics = topics;
            return this;
        }

        public string GetExtensions() {
            return extensions;
        }

        public CollectionData SetExtensions(string extensions) {
            this.extensions = extensions;
            return this;
        }


    }

}

