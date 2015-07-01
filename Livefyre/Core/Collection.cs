using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Core
{
    class Collection
    {

        public class Collection implements LfCore {
        private Site site;
        private CollectionData data;
    
        public Collection(Site site, CollectionData data) {
            this.site = site;
            this.data = data;
        }
    
        public static Collection init(Site site, CollectionType type, String title, String articleId, String url) {
            CollectionData data = new CollectionData(type, title, articleId, url);
            return new Collection(site, ReflectiveValidator.validate(data));
        }
    
        /**
         * Informs Livefyre to either create or update a collection based on the attributes of this Collection.
         * Makes an external API call. Returns this.
         * 
         * @return Collection
         */
        public Collection createOrUpdate() {
            ClientResponse response = invokeCollectionApi("create");
            if (response.getStatus() == 200) {
                data.setId(LivefyreUtil.stringToJson(response.getEntity(String.class))
                        .getAsJsonObject("data").get("collectionId").getAsString());
                return this;
            } else if (response.getStatus() == 409) {
                response = invokeCollectionApi("update");
                if (response.getStatus() == 200) {
                    data.setId(LivefyreUtil.stringToJson(response.getEntity(String.class))
                            .getAsJsonObject("data").get("collectionId").getAsString());
                    return this;
                }
            }
            throw new ApiException(response.getStatus());
        }

        /**
         * Generates a collection meta token representing this collection.
         * 
         * @return String.
         */
        public String buildCollectionMetaToken() {
            Map<String, Object> claims = data.asMap();
            boolean isNetworkIssued = isNetworkIssued();
            claims.put("iss", isNetworkIssued ? site.getNetwork().getUrn() : site.getUrn());
            return LivefyreUtil.serializeAndSign(claims, isNetworkIssued ?
                    site.getNetwork().getData().getKey() : site.getData().getKey());
        }

        /**
         * Generates a MD5-encrypted checksum based on this collection's attributes.
         * 
         * @return String.
         */
        public String buildChecksum() {
            try {
                Map<String, Object> attr = data.asMap();
                byte[] digest = MessageDigest.getInstance("MD5").digest(LivefyreUtil.mapToJsonString(attr).getBytes());
                return printHexBinary(digest);
            } catch (NoSuchAlgorithmException e) {
                throw new LivefyreException("MD5 message digest missing. This shouldn't ever happen." + e);
            }
        }

        /**
         * Retrieves this collection's information from Livefyre. Makes an external API call.
         * 
         * @return JSONObject.
         */
        public JsonObject getCollectionContent() {
            String b64articleId = Base64Url.encode(data.getArticleId().getBytes());
            if (b64articleId.length() % 4 != 0) {
                b64articleId = b64articleId + StringUtils.repeat("=", 4 - (b64articleId.length() % 4));
            }
            String url = String.format("%s/bs3/%s.fyre.co/%s/%s/init", Domain.bootstrap(this), site.getNetwork().getNetworkName(), site.getData().getId(), b64articleId);

            ClientResponse response = Client.create().resource(url).accept(MediaType.APPLICATION_JSON)
                    .get(ClientResponse.class);
            if (response.getStatus() >= 400) {
                throw new ApiException(response.getStatus());
            }
            Gson gson = new Gson();
            return gson.fromJson(response.getEntity(String.class), JsonObject.class);
        }

        public String getUrn() {
            return String.format("%s:collection=%s", site.getUrn(), data.getId());
        }
    
        public boolean isNetworkIssued() {
            List<Topic> topics = data.getTopics();
            if (topics == null || topics.isEmpty()) {
                return false;
            }

            String networkUrn = site.getNetwork().getUrn();
            for (Topic topic : topics) {
                String topicId = topic.getId();
                if (topicId.startsWith(networkUrn) && !topicId.replace(networkUrn, "").startsWith(":site=")) {
                    return true;
                }
            }
            return false;
        }

        public Site getSite() {
            return site;
        }

        public void setSite(Site site) {
            this.site = site;
        }

        public CollectionData getData() {
            return data;
        }
    
        public void setData(CollectionData data) {
            this.data = data;
        }

        private ClientResponse invokeCollectionApi(String method) {
            String uri = String.format("%s/api/v3.0/site/%s/collection/%s/", Domain.quill(this), site.getData().getId(), method);
            ClientResponse response = Client.create().resource(uri).queryParam("sync", "1")
                    .accept(MediaType.APPLICATION_JSON).type(MediaType.APPLICATION_JSON)
                    .post(ClientResponse.class, getPayload());
            return response;
        }
    
        private String getPayload() {
            Map<String, Object> payload = ImmutableMap.<String, Object>of(
                "articleId", data.getArticleId(),
                "checksum", buildChecksum(),
                "collectionMeta", buildCollectionMetaToken());
            return LivefyreUtil.mapToJsonString(payload);
        }

        private static final char[] hexCode = "0123456789abcdef".toCharArray();

        private String printHexBinary(byte[] data) {
            StringBuilder r = new StringBuilder(data.length * 2);
            for (byte b : data) {
                r.append(hexCode[(b >> 4) & 0xF]);
                r.append(hexCode[(b & 0xF)]);
            }
            return r.toString();
        }
    }

        // PULLED FROM SITE.cs
        // turn into CollectionBuilder?
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
