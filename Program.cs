using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livefyre.Core;
using Livefyre;


namespace ExecuteLivefyreLib
{
    class Program
    {
        static void Main(string[] args)
        {
            // Options/Config
            string networkName = "";
            string networkKey = "";
            
            string siteID = "";
            string siteKey = "";

            string userID = "";
            string displayName = "";
            double expires = 0;

            string userSyncURL = "";


            string blogTitle = "";



            Network network = Livefyre.Livefyre.GetNetwork(networkName, networkKey);

            //update a Network's name and key 
            network.GetData().SetName("example-qa-fyre.co");
            network.GetData().SetKey("exampleqabase64key");
        
            //Set SSL off
            network.SetSsl(false);
        
            //Get system and user tokens
            String systemToken = network.BuildLivefyreToken();
            String userToken = network.BuildUserAuthToken("guest", "SuperGuest", 100000.0);
        
            //make sure a system token is still valid
            Boolean isValid = network.ValidateLivefyreToken(systemToken);
        
            //Get the network URN
            String networkUrn = network.GetUrn();
        
            //Get the URN for a specific user
            String userUrn = network.GetUrnForUser("guest");
        
            //Ping for Pull (Set URL then sync user afterwards)
            network.SetUserSyncUrl("www.example-qa.com/user/{id}");
            network.SyncUser("guest");
        
            //Get a Site class
            Site site = network.GetSite("100", "examplesite100base64key");
        
            //update a Site's data
            site.GetData().SetId("101");
            site.GetData().SetKey("examplesite101base64key");
        
            //Get the Site's URN
            String siteUrn = site.GetUrn();
        
            //build a Live Blog Collection
            Collection blogCollection = site.BuildBlogCollection("my blog!", "blog01", "www.example-qa.com/blog01");
            blogCollection.GetData().SetTags("superb");
            blogCollection.CreateOrUpdate();
        
            blogCollection.GetData().SetTags("superb, awesome");
            blogCollection.CreateOrUpdate();
        
            //build a Comments Collection
            Collection commentsCollection = site.buildCommentsCollection("my comments!", "comments01", "www.example-qa.com/comments01");
            commentsCollection.GetData().SetExtensions("{\"something\":\"extra\"}");
            commentsCollection.CreateOrUpdate();
        
            //build a Chat Collection and retrieve content info
            Collection chatCollection = site.buildChatCollection("my chat!", "chat01", "www.example-qa.com/chat01");
        
            try {
                chatCollection.GetCollectionContent();

            }
            //catch (ApiException e) { 
            catch (Exception e) {

                Console.WriteLine(String.Format("LOG: can't retrieve content of a collection that has not been created! \n {0} \n", e) );
            }
        
            // this would be the point of ILMerge
            JObject chatCollectionData = chatCollection.CreateOrUpdate().GetCollectionContent();
        
            //build a Sidenotes Collection and create checksum/token/URN
            Collection sidenotesCollection = site.buildSidenotesCollection("my sidenotes!", "sidenotes01", "www.example-qa.com/sidenotes01");
            String checksum = sidenotesCollection.buildChecksum();
            String token = sidenotesCollection.buildCollectionMetaToken();
            sidenotesCollection.createOrUpdate();
        
            //createOrUpdate must be called to Get an ID for sidenotesCollection.
            String collectionUrn = sidenotesCollection.GetUrn();

        }
    }
}
