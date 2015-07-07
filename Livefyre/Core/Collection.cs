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
using Livefyre.Dto;
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
        public string BuildCollectionMetaToken() {
            //convert to Dict
            Map<String, Object> claims = data.asMap();
            boolean isNetworkIssued = isNetworkIssued();
            claims.put("iss", isNetworkIssued ? site.GetNetwork().GetUrn() : site.GetUrn());
            return LivefyreUtil.serializeAndSign(claims, isNetworkIssued ?
                    site.GetNetwork().GetData().GetKey() : site.GetData().GetKey());
        }
        */
        /**
         * Generates a MD5-encrypted checksum based on this collection's attributes.
         * 
         * @return String.
         */
        /*
        public string BuildChecksum() {
            try {
                // more dictionary-ing
                Map<String, Object> attr = data.asMap();
                byte[] digest = MessageDigest.GetInstance("MD5").digest(LivefyreUtil.mapToJsonString(attr).GetBytes());
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
        
        
        public JObject GetCollectionContent() {
            string articleID = this.data.GetArticleId();
            byte[] bytesID = Encoding.ASCII.GetBytes(articleID);

            string b64articleId = Convert.ToBase64String(bytesID);

            if (b64articleId.Length % 4 != 0) {
                int lengthMod = (b64articleId.Length % 4);
                int diff = (4 - lengthMod);
                string padding = (string)Enumerable.Repeat('=', diff);
                string suffix = string.Concat("", padding);

                b64articleId = b64articleId + suffix;
            }

            // make configgleable
            string url = String.Format("{0}/bs3/{1}.fyre.co/{2}/{3}/init", 
                Domain.bootstrap(this), this.site.GetNetwork().GetNetworkName(), 
                    this.site.GetData().GetId(), b64articleId);

            
            WebRequest request = WebRequest.Create(url);
            request.ContentType = "application/json";

            // USER AGENT MAY BE NECESSARY!
            //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";
                

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            response.Close();

            // responseCode check - make Utils method
            // 300s wont work here either
            // should be 200 or ERROR?
            // if ((int)response.StatusCode !== 200) {
            if ((int)response.StatusCode >= 400) {
                // make LF Exception
                //throw new ApiException(response.GetStatus());
                throw new Exception(String.Format("An error has occured: {0}", response.StatusCode));

            }


            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);

            string responseString = responseReader.ReadToEnd();

            responseReader.Close();
            responseStream.Close();

            // is String.class helping to create the actual Java type?
            return new JObject(responseString);
        }
        

        
        public string GetUrn() {
            return String.Format("{0}:collection={1}", this.site.GetUrn(), this.data.GetId());
        }


        public bool IsNetworkIssued() {
            List<Topic> topics = data.GetTopics();

            int l = topics.Count;

            if (topics == null || l == 0) {
                return false;
            }

            string netURN = this.site.GetNetwork().GetUrn();
            int i = 0;

            Topic t;

            for (; i < l; i += 1)
            {
                t = topics[i];
                string ID = t.GetId();

                // STRING FRAG - pull out to config!
                //SAFE!? -- if one responds, return true
                if (ID.StartsWith(netURN) && !(ID.Replace(netURN, "").StartsWith(":site=")))
                {
                    return true;
                }
            }

            return false;
        }


        public Site GetSite() {
            return this.site;
        }

        public void SetSite(Site site) {
            this.site = site;
        }

        public CollectionData GetData() {
            return this.data;
        }
    
        public void SetData(CollectionData data) {
            this.data = data;
        }

        

        // PRIVATE
            
        private HttpWebResponse InvokeCollectionApi(string method) {
            // string frag - pull to config object
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
        private string GetPayload() {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            
            payload.Add("articleId", data.GetArticleId());
            payload.Add("checksum", BuildChecksum);
            payload.Add("articleId", data.BuildCollectionMetaToken);
            
            return TypeToJson(payload);
        }
        */
        // check this
        /*
        private string PrintHexBinary(byte[] data) {
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
