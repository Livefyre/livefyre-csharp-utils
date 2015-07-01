using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics.Contracts;

using Livefyre.Model;
using Livefyre.Utils;

namespace Livefyre.Core
{
    public class Network
    {
        private static double DEFAULT_EXPIRES = 86000.0;
        private static string DEFAULT_USER = "system";
        //check this
        private static string ID = "{id}";
        private static string ALPHA_DASH_UNDER_DOT_REGEX = "^[a-zZA-Z0-9_\\.-]+$";

        private NetworkData data;
        private bool ssl = true;


        public Network(NetworkData data)
        {
            // get/set?
            this.data = data;
        }
        // MOVE THIS TO CONTRUCTOR?

        //creating second object on init
        public static Network Init(String name, String key)
        {
            NetworkData data = new NetworkData(name, key);
            return new Network(data/* David: not nec to use reflection for Validation here */);
        }

        /**
           * Updates the user sync url. Makes an external API call. 
           * http://answers.livefyre.com/developers/user-auth/remote-profiles/#ping-for-pull.
           * 
           * @param urlTemplate the url template to set.
        */
        /**
         * Updates the user sync url. Makes an external API call. 
         * http://answers.livefyre.com/developers/user-auth/remote-profiles/#ping-for-pull.
         * 
         * @param urlTemplate the url template to set.
         */
        public void SetUserSyncUrl(String url)
        {
            Precondition.CheckNotNull(url, String.Format("urlTemplate does not contain {0}", ID));

            try
            {
                // fix ref
                String postData = String.Format("{0}", Domain.quill(this));
                // fix ref
                postData = String.Format(postData + "{0}", BuildLivefyreToken());
                //add Params
                // make params vars/members
                // actor_token
                // pull_profile_url
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);

                Uri uri = new Uri(url);
                WebRequest request = WebRequest.Create(uri);
                request.ContentType = "application/x-www-form-urlencoded";

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(postBytes, 0, postBytes.Length);
                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                Console.WriteLine(responseFromServer);
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        /**
         * Informs Livefyre to fetch user information based on the user sync url. Makes an external API call.
         * 
         * @param userId the userId for the user to sync
         * @return true if the sync was successful.
         */
        public Network SyncUser(String userId)
        {
            Precondition.CheckNotNull(userId);

            //fix ref
            //make configurable/pull out mutable api v3_0 key
            string url = String.Format("{0}/api/v3_0/user/{1}/refresh", Domain.quill(this), userId);
            // pull bits out of try
            try
            {
                // fix ref
                string postData = String.Format("{0}", Domain.quill(this));
                // fix ref
                postData = String.Format(postData + "{0}", this.buildLivefyreToken());
                //add Params
                // make params vars/members
                // lftoken
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);

                Uri uri = new Uri(url);
                WebRequest request = WebRequest.Create(uri);
                request.ContentType = "application/x-www-form-urlencoded";

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(postBytes, 0, postBytes.Length);
                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                Console.WriteLine(responseFromServer);
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();


                // capture/throw a server error code too

                return this;
            }
            catch (Exception e)
            {
                throw e;
            }

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
         * and signs the String with the Network key.
         * 
         * @param userId the user id for this token.
         * @param displayName the display name for this token.
         * @param expires when the token should expire from the time of its creation in seconds.
         * @return String
         */
        public String BuildUserAuthToken(String userId, String displayName, Double expires)
        {
            Regex pattern = new Regex(ALPHA_DASH_UNDER_DOT_REGEX);
            // should we use ms code contracts?
            // requires a runtime checking setting
            Precondition.CheckNotNull(userId);
            Precondition.CheckNotNull(displayName);
            Precondition.CheckNotNull(expires);

            // var/mem this error message
            if (pattern.IsMatch(userId))
            {

                /*
                // change this to MS type
                Map<String, Object> claims = ImmutableMap.<String, Object>of(
                        "domain", data.getName(),
                        "user_id", userId,
                        "display_name", displayName,
                        "expires", getExpiryInSeconds(expires)
                    );

                return SerializeAndSign(claims, data.getKey());
                */
                return "";

            }
            else
            {
                throw new Exception(String.Format("userId is not valid. be sure the userId matches the following pattern: {0}",
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
        public bool ValidateLivefyreToken(String lfToken)
        {
            Precondition.CheckNotNull(lfToken);

            // JSON.Net http://www.newtonsoft.com/json lib or MS JSON Serializer?

            /*
            JsonObject json = LivefyreUtil.decodeJwt(lfToken, data.getKey());
            return json.get("domain").getAsString().compareTo(data.getName()) == 0
                && json.get("user_id").getAsString().compareTo("system") == 0
                && json.get("expires").getAsLong() >= Calendar.getInstance().getTimeInMillis()/1000L;
             */
            return false;
        }


        public string GetUrn()
        {
            // ensure this structure in networkdata
            return "urn:livefyre:" + data.GetName();
        }

        public String getUrnForUser(String user)
        {
            return GetUrn() + ":user=" + user;
        }

        public String getNetworkName()
        {
            // ensure this structure in networkdata
            return data.GetName().split("\\.")[0];
        }

        public Boolean isSsl()
        {
            return ssl;
        }


        // get/set
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


        /* Protected/private methods */
        // move to Util
        private long getExpiryInSeconds(double secTillExpire)
        {
            //MS Time
            /*
            Calendar cal = Calendar.getInstance(TimeZone.getTimeZone("UTC"));
            cal.add(Calendar.SECOND, (int) secTillExpire);
            return cal.getTimeInMillis() / 1000L;
            */
            return 199430;
        }
    }
}

