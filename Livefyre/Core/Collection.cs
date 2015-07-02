﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Model;
using Livefyre.Type;

namespace Livefyre.Core
{
    public class Collection : LFCore
    {

        private Site site;
        private CollectionData data;
        private static char[] hexCode = "0123456789abcdef".ToCharArray();
    
        public Collection(Site site, CollectionData data) {
            this.site = site;
            this.data = data;
        }
    
        public static Collection Init(Site site, CollectionType type, String title, String articleId, String url) {
            CollectionData data = new CollectionData(type, title, articleId, url);
            return new Collection(site, ReflectiveValidator.validate(data));
        }
    
        /**
         * Informs Livefyre to either create or update a collection based on the attributes of this Collection.
         * Makes an external API call. Returns this.
         * 
         * @return Collection
         */
        public Collection CreateOrUpdate() {

            //request may be able to be refactored into util method

            string response = InvokeCollectionApi("create");


            // make request, check status
            // make status parse method in utils
            if (response.getStatus() == 200) {
                data.SetId(LivefyreUtil.stringToJson(response.getEntity(String.class))
                        .GetAsJsonObject("data").get("collectionId").GetAsString());
                return this;


            } else if (response.getStatus() == 409) {
                response = InvokeCollectionApi("update");
                if (response.getStatus() == 200) {
                    data.setId(LivefyreUtil.stringToJson(response.getEntity(String.class))
                            .GetAsJsonObject("data").get("collectionId").GetAsString());
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
        public String BuildCollectionMetaToken() {
            //convert to Dict
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
        public String BuildChecksum() {
            try {
                // more dictionary-ing
                Map<String, Object> attr = data.asMap();
                byte[] digest = MessageDigest.getInstance("MD5").digest(LivefyreUtil.mapToJsonString(attr).getBytes());
                return printHexBinary(digest);
            } catch (NoSuchAlgorithmException e) {
            // pull out these error strings. /make configurable some day?
                throw new LivefyreException("MD5 message digest missing. This shouldn't ever happen." + e);
            }
        }

        /**
         * Retrieves this collection's information from Livefyre. Makes an external API call.
         * 
         * @return JSONObject.
         */
        public JsonObject GetCollectionContent() {
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

        public String GetUrn() {
            return String.format("%s:collection=%s", site.getUrn(), data.getId());
        }
    
        public boolean IsNetworkIssued() {
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

        public Site GetSite() {
            return site;
        }

        public void SetSite(Site site) {
            this.site = site;
        }

        public CollectionData GetData() {
            return data;
        }
    
        public void SetData(CollectionData data) {
            this.data = data;
        }


        // PULLED FROM SITE.cs
        // turn into CollectionBuilder?
        /* Default collection type */
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
            return Collection.init(this, type, title, articleId, url);
        }



        // PRIVATE

        //MAKE WEBCLIENT CALL
        private ClientResponse InvokeCollectionApi(String method) {
             Uri uri = new Uri(String.Format("{0}/api/v3.0/site/{1}/collection/{2}/", Domain.quill(this), site.GetData().GetId(), method));

            // add func: PARSE return code on returned value
            
            //REFACTOR REQUEST INTO UTIL METHOD?

            string postData = "sync=1";
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);
            // ascii or utf8?
            // Apply ASCII Encoding to obtain the string as a byte array. 
            // byte[] postBytes = Encoding.ASCII.GetBytes(postData);


            // Create a new WebClient instance.
            WebClient webClient = new WebClient();
            // check these - going to blow up it feels
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            webClient.Headers.Add("Accept", "application/json");
            //  another header here?  webClient.Headers.Add("Accept", "application/json");
            byte[] responseArray = webClient.UploadData(uri, "POST", postBytes);

            Console.WriteLine("Uploading to {0} ...", uri.ToString());

            // make object with GetResponse method
            string response = Encoding.UTF8.GetString(responseArray);
            return response;
        }
    
        private String GetPayload() {

            // Dictionary!
            Map<String, Object> payload = ImmutableMap.<String, Object>of(
                "articleId", data.getArticleId(),
                "checksum", buildChecksum(),
                "collectionMeta", BuildCollectionMetaToken());
            return LivefyreUtil.mapToJsonString(payload);
        }


        private String PrintHexBinary(byte[] data) {
            StringBuilder r = new StringBuilder(data.length * 2);
            for (byte b : data) {
                r.append(hexCode[(b >> 4) & 0xF]);
                r.append(hexCode[(b & 0xF)]);
            }
            return r.toString();
        }




    }
}
