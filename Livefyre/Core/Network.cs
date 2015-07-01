using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Livefyre.Model;
using Livefyre.Utils;

namespace Livefyre.Core
{
    public class Network
    {
        private static double DEFAULT_EXPIRES = 86000.0;
        private static string DEFAULT_USER = "system";
        private static string ID = "{id}";
        private static string ALPHA_DASH_UNDER_DOT_REGEX = "^[a-zZA-Z0-9_\\.-]+$";

        private NetworkData data;
        private bool ssl = true;


        public Network(NetworkData data)
        {
            // get/set?
            this.data = data;
        }

        public static Network Init(String name, String key) {
            NetworkData data = new NetworkData(name, key);
            return new Network(data/* David: not nec to use reflection here if desired */);
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
    public void SetUserSyncUrl(String url) {
        Precondition.CheckNotNull(url, String.Format("urlTemplate does not contain {0}", ID));

        try 
	    {	        
            // fix ref
            String postData = String.Format("{0}", Domain.quill(this));
            // fix ref
            postData = String.Format(postData + "{0}", this.buildLivefyreToken());
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
            Console.WriteLine (((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader (dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine (responseFromServer);
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
    public Network SyncUser(String userId) {
        Precondition.CheckNotNull(userId);
        
        //fix ref
        //make configurable/pull out mutable api v3_0 key
        string url = String.Format("{0}/api/v3_0/user/{1}/refresh", Domain.quill(this), userId);

        try 
	    {	        
            // fix ref
            String postData = String.Format("{0}", Domain.quill(this));
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
            Console.WriteLine (((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader (dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine (responseFromServer);
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

 
        return this;
    }
    
    /**
     * Generates a user auth system token.
     * 
     * @return a default system token
     */
    public string buildLivefyreToken() {
        return buildUserAuthToken(DEFAULT_USER, DEFAULT_USER, DEFAULT_EXPIRES);
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
    public String buildUserAuthToken(String userId, String displayName, Double expires) {
        Pattern pattern = Pattern.compile(ALPHA_DASH_UNDER_DOT_REGEX);
        checkArgument(pattern.matcher(CheckNotNull(userId)).find(),
                "userId is not valid. be sure the userId matches the following pattern: %s", ALPHA_DASH_UNDER_DOT_REGEX);
        CheckNotNull(displayName);
        CheckNotNull(expires);

        Map<String, Object> claims = ImmutableMap.<String, Object>of(
                "domain", data.getName(),
                "user_id", userId,
                "display_name", displayName,
                "expires", getExpiryInSeconds(expires)
            );

        return LivefyreUtil.serializeAndSign(claims, data.getKey());
    }

    /**
     * Checks to see if the passed in system token is still valid.
     * 
     * @param lfToken the system token to validate
     * 
     * @return true if the token is still valid.
     */
    public boolean validateLivefyreToken(String lfToken) {
        CheckNotNull(lfToken);

        JsonObject json = LivefyreUtil.decodeJwt(lfToken, data.getKey());
        return json.get("domain").getAsString().compareTo(data.getName()) == 0
            && json.get("user_id").getAsString().compareTo("system") == 0
            && json.get("expires").getAsLong() >= Calendar.getInstance().getTimeInMillis()/1000L;
    }
    
    /**
     * Constructs a new Site object based on the parameters passed in.
     * 
     * @param siteId the id for the Site.
     * @param siteKey the secret key for the Site.
     * @return Site
     */
    public Site getSite(String siteId, String siteKey) {
        return Site.init(this, siteId, siteKey);
    }
    
    public String getUrn() {
        return "urn:livefyre:"+data.getName();
    }
    
    public String getUrnForUser(String user) {
        return getUrn()+":user="+user;
    }
    
    public String getNetworkName() {
        return data.getName().split("\\.")[0];
    }

    public Boolean isSsl() {
        return ssl;
    }

    public void setSsl(Boolean ssl) {
        this.ssl = ssl;
    }

    public NetworkData getData() {
        return data;
    }
    
    public void setData(NetworkData data) {
        this.data = data;
    }

    /* Protected/private methods */
    private long getExpiryInSeconds(double secTillExpire) {
        Calendar cal = Calendar.getInstance(TimeZone.getTimeZone("UTC"));
        cal.add(Calendar.SECOND, (int) secTillExpire);
        return cal.getTimeInMillis() / 1000L;
    }
}

    }
}
