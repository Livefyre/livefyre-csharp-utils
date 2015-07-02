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
        private String id;
        private String articleId;
        private String title;
        private String url;
    
        //optional params
        private String tags;
        private List<Topic> topics;
        private String extensions;
    
        public CollectionData(CollectionType type, String title, String articleId, String url) {
            this.type = type;
            this.articleId = articleId;
            this.title = title;
            this.url = url;
        }

        // make C# Dictionary
        public Map<String, Object> AsMap() {

            Map<String, Object> attr = Maps.newTreeMap();
            attr.put("articleId", articleId);
            attr.put("title", title);


            // work out the enum lang conversion puzzle
            attr.put("type", (string)type);
            attr.put("url", url);
        
            if (String.IsNullOrEmpty(tags)) {
                attr.put("tags", tags);
            }
            if (topics != null && topics.Length > 0) {
                attr.put("topics", topics);
            }
            if (String.IsNullOrEmpty(extensions))
            {
                attr.put("extensions", extensions);
            }
            return attr;
        }

        // make C# set/get?
        public CollectionType GetType() {
            return type;
        }
    
        public CollectionData SetType(CollectionType type) {
            this.type = type;
            return this;
        }
    
        public String GetId() {
            if (id == null) {
                // make msg var/mem
                throw new LivefyreException("Id not set. Call createOrUpdate() on the collection to set the id, or manually set it by calling setId(id) on this object.");
            }
            return id;
        }
    
        public CollectionData SetId(String id) {
            this.id = id;
            return this;
        }

        public String GetArticleId() {
            return articleId;
        }

        public CollectionData SetArticleId(String articleId) {
            this.articleId = articleId;
            return this;
        }

        public String GetTitle() {
            return title;
        }

        public CollectionData SetTitle(String title) {
            this.title = title;
            return this;
        }

        public String GetUrl() {
            return url;
        }

        public CollectionData SetUrl(String url) {
            this.url = url;
            return this;
        }

        public String GetTags() {
            return tags;
        }

        public CollectionData SetTags(String tags) {
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

        public String GetExtensions() {
            return extensions;
        }

        public CollectionData SetExtensions(String extensions) {
            this.extensions = extensions;
            return this;
        }


    }

}

