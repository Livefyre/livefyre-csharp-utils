using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Livefyre.Api;
using Livefyre.Core;
using Livefyre.Model;
using Livefyre.Type;
using Livefyre.Utils;


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
    
        public static Collection Init(Site site, CollectionType type, string title, string articleId, string url) {
            CollectionData data = new CollectionData(type, title, articleId, url);
            // validate data
            // return new Collection(site, ReflectiveValidator.validate(data));
            return new Collection(site, data);
        }
    
        /**
         * Informs Livefyre to either create or update a collection based on the attributes of this Collection.
         * Makes an external API call. Returns this.
         * 
         * @return Collection
         */
        public Collection CreateOrUpdate() {
            HttpWebResponse response = InvokeCollectionApi("create");
            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);

            string responseString = responseReader.ReadToEnd(); 
                
            responseReader.Close();
            responseStream.Close();


            if ((int)response.StatusCode == 200) {

                JObject json = JObject.Parse(responseString);

                this.data.SetId((string)json["collectionId"]);

                return this;


            } else if ((int)response.StatusCode == 409) {
                response = InvokeCollectionApi("update");

                if ((int)response.StatusCode == 200) {

                    JObject json = JObject.Parse(responseString);

                    this.data.SetId((string)json["collectionId"]);

                    return this;

                }
            }

            // fill in from Custom Exceptions
            //throw new ApiException((int)response.StatusCode);
            throw new Exception(String.Format("An Error has occurred: {0}", (int)response.StatusCode));
            
        }

        /**
         * Generates a collection meta token representing this collection.
         * 
         * @return String.
         */
            /*
        public String BuildCollectionMetaToken() {
            //convert to Dict
            Map<String, Object> claims = data.asMap();
            boolean isNetworkIssued = isNetworkIssued();
            claims.put("iss", isNetworkIssued ? site.getNetwork().getUrn() : site.getUrn());
            return LivefyreUtil.serializeAndSign(claims, isNetworkIssued ?
                    site.getNetwork().getData().getKey() : site.getData().getKey());
        }
        */
        /**
         * Generates a MD5-encrypted checksum based on this collection's attributes.
         * 
         * @return String.
         */
        /*
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
        */
        /**
         * Retrieves this collection's information from Livefyre. Makes an external API call.
         * 
         * @return JSONObject.
         */
        /*
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
        */

        
        public String GetUrn() {
            return String.Format("{0}:collection={1}", this.site.GetUrn(), this.data.GetId());
        }
    
        /*
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
        */
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

        

        // PRIVATE
            
        private HttpWebResponse InvokeCollectionApi(string method) {
             Uri uri = new Uri(String.Format("{0}/api/v3.0/site/{1}/collection/{2}/", Domain.quill(this), site.GetData().GetId(), method));

            // REFACTOR REQUEST INTO UTIL METHOD?
            // WebClient doesn't allow coercion to response object!?
            // using WebRequest instead

            string postData = "sync=1";
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);
            // ascii or utf8?
            // byte[] postBytes = Encoding.ASCII.GetBytes(postData);

            WebRequest request = WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.ContentLength = postBytes.Length;
            request.Method = "POST";

            // USER AGENT MAY BE NECESSARY!
            //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";
                
            // inject Post Data
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);

            requestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            response.Close();

            return response;

        }

    /*
        private String GetPayload() {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            
            payload.Add("articleId", data.GetArticleId());
            payload.Add("checksum", BuildChecksum);
            payload.Add("articleId", data.BuildCollectionMetaToken);
            
            return TypeToJson(payload);
        }
        */
        // check this
        /*
        private String PrintHexBinary(byte[] data) {
            StringBuilder r = new StringBuilder(data.length * 2);
            for (byte b : data) {
                r.append(hexCode[(b >> 4) & 0xF]);
                r.append(hexCode[(b & 0xF)]);
            }
            return r.toString();
        }
        */



    }
}
