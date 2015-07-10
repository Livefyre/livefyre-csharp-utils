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
            string networkName = "example.fyre.co";
            string networkKey = "exampleprodbase64key";

            string updatedNetworkName = "example-qa-fyre.co";
            string updatedNetworkKey = "exampleqabase64key";

            string siteID = "";
            string siteKey = "";

            string updatedSiteID = "";
            string updatedSiteKey = "";


            string userID = "";
            string displayName = "";
            double expires = 0;

            string userSyncURL = "www.example-qa.com/user/{id}";
            
            string blogTitle = "";
            string blogID = "";
            string blogURL = "";
            string blogTag1 = "";
            string blogTag2 = "superb, awesome";


            string commentsTitle = "";
            string commentsID = "";
            string commentsURL = "";
            string commentsExtensionsJSON = "{\"something\":\"extra\"}";

            string chatTitle = "";
            string chatID = "";
            string chatURL = "";

            string sideNotesTitle = "";
            string sideNotesID = "";
            string sideNotesURL = "";



            Network network = Livefyre.Livefyre.GetNetwork(networkName, networkKey);

            //update a Network's name and key 
            network.GetData().SetName(updatedNetworkName);
            network.GetData().SetKey(updatedNetworkKey);
        
            //Set SSL off
            network.SetSsl(false);
        
            //Get system and user tokens
            String systemToken = network.BuildLivefyreToken();
            String userToken = network.BuildUserAuthToken(userID, displayName, expires);
        
            //make sure a system token is still valid
            Boolean isValid = network.ValidateLivefyreToken(systemToken);
        
            //Get the network URN
            String networkUrn = network.GetUrn();
        
            //Get the URN for a specific user
            String userUrn = network.GetUrnForUser(userID);
        
            //Ping for Pull (Set URL then sync user afterwards)
            network.SetUserSyncUrl(userSyncURL);
            network.SyncUser(userID);
        
            //Get a Site class
            Site site = network.GetSite(siteID, siteKey);
        
            //update a Site's data
            site.GetData().SetId(updatedSiteID);
            site.GetData().SetKey(updatedSiteKey);
        
            //Get the Site's URN
            String siteUrn = site.GetUrn();
        
            //build a Live Blog Collection
            Collection blogCollection = site.BuildBlogCollection(blogTitle, blogID, blogURL);
            blogCollection.GetData().SetTags(blogTag1);
            blogCollection.CreateOrUpdate();
        
            blogCollection.GetData().SetTags(blogTag2);
            blogCollection.CreateOrUpdate();
        
            //build a Comments Collection
            Collection commentsCollection = site.buildCommentsCollection(commentsTitle, commentsID, commentsURL);
            commentsCollection.GetData().SetExtensions(commentsExtensionsJSON);
            commentsCollection.CreateOrUpdate();
        
            //build a Chat Collection and retrieve content info
            Collection chatCollection = site.buildChatCollection(chatTitle, chatID, chatURL);
        
            try {
                chatCollection.GetCollectionContent();

            }
            //catch (ApiException e) { 
            catch (Exception e) {

                Console.WriteLine(String.Format("LOG: can't retrieve content of a collection that has not been created! \n {0} \n", e) );
            }
        
            // this would be the point of ILMerge
            //JObject chatCollectionData = chatCollection.CreateOrUpdate().GetCollectionContent();
            string chatCollectionData = chatCollection.CreateOrUpdate().GetCollectionContent();
        
            //build a Sidenotes Collection and create checksum/token/URN
            Collection sidenotesCollection = site.buildSidenotesCollection(sideNotesTitle, sideNotesID, sideNotesURL);
            String checksum = sidenotesCollection.BuildChecksum();
            String token = sidenotesCollection.BuildCollectionMetaToken();
            sidenotesCollection.CreateOrUpdate();
        
            //createOrUpdate must be called to Get an ID for sidenotesCollection.
            String collectionUrn = sidenotesCollection.GetUrn();

        }
    }
}
