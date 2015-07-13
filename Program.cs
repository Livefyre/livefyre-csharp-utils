using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livefyre.Core;
using Livefyre;

using JWT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace ExecuteLivefyreLib
{
    class Program
    {
        static void Main(string[] args)
        {
            // Options/Config
            string networkName = "mwebb-integration.fyre.co";


            // PARSE ARGS? LOOK FOR SHELL VARS?

            
            // SECURE ME!
            // BAD FOR REPOS!
            string networkKey = "Mr7p8Pfx1rR7cC2bgLTIeyO+nBQ=";

            string updatedNetworkName = "mwebb-integration.fyre.co";

            // SECURE ME!
            // BAD FOR REPOS!
            string updatedNetworkKey = "Mr7p8Pfx1rR7cC2bgLTIeyO+nBQ=";

            string siteID = "305377";

            // SECURE ME!
            // BAD FOR REPOS!
            string siteKey = "E+1EdBnTf9NvcuFjgRAh2kNX2qo=";

            string updatedSiteID = "305377";

            // SECURE ME!
            // BAD FOR REPOS!
            string updatedSiteKey = "E+1EdBnTf9NvcuFjgRAh2kNX2qo=";

            string userID = "71037700";
            string displayName = "mwebb";

            // pad 30 days
            DateTime expiration = DateTime.UtcNow.AddDays(30);
            double expires = (Int32)(expiration.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                
            string userSyncURL = "www.groovyverse.com/user/{id}";
            
            string blogTitle = "My First Blog";
            string blogID = "UNIQUE-ID-59487632945749";
            string blogURL = "www.groovyverse.com/blog";
            string blogTag1 = "tag1";
            string blogTag2 = "tag2, superb, awesome";

            string commentsTitle = "My First Comment";
            string commentsID = "UNIQUE-ID-497397r98efhisudhf95y";
            string commentsURL = "www.groovyverse.com/comments";
            string commentsExtensionsJSON = "{\"something\":\"extra\"}";

            string chatTitle = "My First Chat";
            string chatID = "UNIQUE-ID-805732495873FHSKLFJSDF";
            string chatURL = "www.groovyverse.com/chat";

            string sideNotesTitle = "My First Sidenotes";
            string sideNotesID = "UNIQUE-ID-FJKHASKDJFHASLHRR9587934875KGHJ";
            string sideNotesURL = "www.groovyverse.com/sidenotes";



            Network network = Livefyre.Livefyre.GetNetwork(networkName, networkKey);

            //update a Network's name and key 
            network.GetData().SetName(updatedNetworkName);
            network.GetData().SetKey(updatedNetworkKey);
        
            //Set SSL off
            network.SetSsl(false);
        
            //Get system and user tokens
            string systemToken = network.BuildLivefyreToken();
            string userToken = network.BuildUserAuthToken(userID, displayName, expires);
        
            //make sure a system token is still valid
            bool isValid = network.ValidateLivefyreToken(systemToken);
        
            //Get the network URN
            string networkUrn = network.GetUrn();
        
            //Get the URN for a specific user
            string userUrn = network.GetUrnForUser(userID);
        
            //Ping for Pull (Set URL then sync user afterwards)
            network.SetUserSyncUrl(userSyncURL);
            network.SyncUser(userID);
        
            //Get a Site class
            Site site = network.GetSite(siteID, siteKey);
        
            //update a Site's data
            site.GetData().SetId(updatedSiteID);
            site.GetData().SetKey(updatedSiteKey);
        
            //Get the Site's URN
            string siteUrn = site.GetUrn();
        
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
            JObject chatCollectionData = chatCollection.CreateOrUpdate().GetCollectionContent();
        
            //build a Sidenotes Collection and create checksum/token/URN
            Collection sidenotesCollection = site.buildSidenotesCollection(sideNotesTitle, sideNotesID, sideNotesURL);
            string checksum = sidenotesCollection.BuildChecksum();
            string token = sidenotesCollection.BuildCollectionMetaToken();
            sidenotesCollection.CreateOrUpdate();
        
            //createOrUpdate must be called to Get an ID for sidenotesCollection.
            string collectionUrn = sidenotesCollection.GetUrn();

        }
    }
}
