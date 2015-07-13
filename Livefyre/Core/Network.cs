using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

using Livefyre.Api;
using Livefyre.Model;
using Livefyre.Utils;
using Livefyre.Validators;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Livefyre.Core
{
    public class Network : LFCore
    {
        private static readonly double DEFAULT_EXPIRES = 86000.0;
        private static readonly string DEFAULT_USER = "system";
        private static readonly string ID = "{id}";
        private static readonly string ALPHA_DASH_UNDER_DOT_REGEX = "^[a-zZA-Z0-9_\\.-]+$";

        private NetworkData data;

        private bool SSL = true;


        public Network(NetworkData data)
        {
            this.data = data;
        }
        // MOVE THIS TO CONTRUCTOR?

        //creating second object on init
        public static Network Init(string name, string key)
        {
            NetworkData data = new NetworkData(name, key);

            NetworkValidator validator = new NetworkValidator();

            return new Network(validator.Validate(data));
        }

        /**
           * Updates the user sync url. Makes an external API call. 
           * http://answers.livefyre.com/developers/identity-integration/your-identity/#PingForPull
           * 
           * @param urlTemplate the url template to Set.
        */
        public void SetUserSyncUrl(string urlTemplate)
        {
            if (urlTemplate.IndexOf(ID) == -1)
            {
                // make LF Exception
                throw new Exception(string.Format("urlTemplate does not contain {0}", ID));
            }

            //REFACTOR REQUEST INTO UTIL METHOD?
            // make params vars/members?

            // USER AGENT MAY BE NECESSARY!
            //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";

            try {
                using(WebClient client = new WebClient()) {

                    // what about 'Accepts JSON' here?

                    NameValueCollection postParams = new NameValueCollection() { 
                       { "actor_token", BuildLivefyreToken() },
                       { "pull_profile", urlTemplate }
                    };

                    Uri uri = new Uri(string.Format("{0}", Domain.quill(this)));
                    byte[] response = client.UploadValues(uri, postParams);
                    string result = Encoding.UTF8.GetString(response);

                }


            } catch (Exception) {
                throw;
            }
       
        }


        /**
         * Informs Livefyre to fetch user information based on the user sync url. Makes an external API call.
         * 
         * @param userId the userId for the user to sync
         * @return true if the sync was successful.
         */
        public Network SyncUser(string userId)
        {
            Precondition.CheckNotNull(userId);

            //make configurable/pull out mutable api v3_0 key
            string url = string.Format("{0}/api/v3_0/user/{1}/refresh", Domain.quill(this), userId);

            //REFACTOR REQUEST INTO UTIL METHOD?

            string postData = string.Format("{0}", Domain.quill(this));
            // make params vars/members
            postData = string.Format(postData + "&lftoken={0}", BuildLivefyreToken());
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);
            // ascii or utf8?
            // byte[] postBytes = Encoding.ASCII.GetBytes(postData);

            // try this?
            Uri uri = new Uri(url);
               
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

            if ((int)response.StatusCode >= 400)
            {

                // make this custom exception
                // throw new ApiException(response.GetStatus());
                throw new Exception(string.Format("An Error has occurred: {0}", (int)response.StatusCode));

            }

            return this;


        }

        /**
         * Generates a user auth system token.
         * 
         * @return a default system token
         */
        public string BuildLivefyreToken()
        {
            return BuildUserAuthToken(DEFAULT_USER, DEFAULT_USER, DEFAULT_EXPIRES);
        }

        /**
         * Generates a user auth token passed on the params passed in. This method serializes the params
         * and signs the string with the Network key.
         * 
         * @param userId the user id for this token.
         * @param displayName the display name for this token.
         * @param expires when the token should expire from the time of its creation in seconds.
         * @return string
         */
        public string BuildUserAuthToken(string userId, string displayName, Double expires)
        {
            // should we use ms code contracts?
            Precondition.CheckNotNull(userId);
            Precondition.CheckNotNull(displayName);
            Precondition.CheckNotNull(expires);

            Regex pattern = new Regex(ALPHA_DASH_UNDER_DOT_REGEX);

            // var/mem this error message
            if (pattern.IsMatch(userId))
            {
                Dictionary<string, object> claims = new Dictionary<string, object>();

                claims.Add("domain", data.GetName());
                claims.Add("user_id", userId);
                claims.Add("display_name", displayName);
                claims.Add("expires", GetExpiryInSeconds(expires));

                return LivefyreUtil.SerializeAndSign(claims, data.GetKey());

            }
            else
            {
                throw new Exception(string.Format("userId is not valid. be sure the userId matches the following pattern: {0}",
                        ALPHA_DASH_UNDER_DOT_REGEX));
            }

        }

        /**
         * Checks to see if the passed in system token is still valid.
         * 
         * @param lfToken the system token to validate
         * 
         * @return true if the token is still valid.
         */
        public bool ValidateLivefyreToken(string lfToken)
        {
            Precondition.CheckNotNull(lfToken);

            string jwt = LivefyreUtil.DecodeJWT(lfToken, data.GetKey());

            JObject jwtObject = JObject.Parse(jwt);

            // check tree
            // so many strings
            string domain = (string)jwtObject["domain"];

            string userID = (string)jwtObject["user_id"];
            long expires = (long)jwtObject["expires"];

            if ( domain.Equals(data.GetName()) &&
                    userID.Equals("system") &&
                    expires >= LivefyreUtil.UnixNow())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /**
         * Constructs a new Site object based on the parameters passed in.
         * 
         * @param siteId the id for the Site.
         * @param siteKey the secret key for the Site.
         * @return Site
         */
        public Site GetSite(string siteId, string siteKey)
        {
            return Site.Init(this, siteId, siteKey);
        }


        public string GetUrn()
        {
            // ensure this structure in networkdata
            return "urn:livefyre:" + this.data.GetName();
        }

        public string GetUrnForUser(string user)
        {
            return GetUrn() + ":user=" + user;
        }

        public string GetNetworkName()
        {
            string netName = this.data.GetName();
            char[] splitChar = { '.' };

            return netName.Split(splitChar)[0];
        }

        public bool IsSsl()
        {
            return SSL;
        }


        // Get/Set
        public void SetSsl(bool SSL)
        {
            this.SSL = SSL;
        }

        public NetworkData GetData()
        {
            return data;
        }

        public void SetData(NetworkData data)
        {
            this.data = data;
        }


        /* Protected/private methods */
        // move to Util
        private long GetExpiryInSeconds(double secTillExpire)
        {
            DateTime expiration = DateTime.UtcNow.AddSeconds(secTillExpire);
            
            return (Int32)(expiration.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}

