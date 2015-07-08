using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Livefyre.Api.Filter;
using Livefyre.Core;
using Livefyre.Dto;
using Livefyre.Type;
using Livefyre.Utils;
using Livefyre.Validator;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace Livefyre.Api
{
    public class PersonalizedStream {

        // check format index positioning
        private static readonly string BASE_URL = "{0}/api/v4";
        private static readonly string STREAM_BASE_URL = "{0}/api/v4";
    
        private static readonly string TOPIC_PATH = "/{0}/";
        private static readonly string MULTIPLE_TOPIC_PATH = "/{0}:topics/";
        private static readonly string USER_SUBSCRIPTION_PATH = "/{0}:subscriptions/";
        private static readonly string TOPIC_SUBSCRIPTION_PATH = "/{0}:subscribers/";
        private static readonly string TIMELINE_PATH = "/timeline/";
    
        private static readonly string PATCH_METHOD = "PATCH";
    


        // REPETITION ON REQUESTS - DRYYYYYYY THIS

        // THE REQUEST METHODS BELOW CAN BE BROKEN INTO SMALLER METHODS:
            // build URI
            // build Headers
            // build POST Data
            // validate response
            // deserializa appropriately


        /* Topic API */

        public static Topic GetTopic(LFCore core, string topicId) {
            Uri baseURI = BuildURL(core);
            Uri completeURI = new Uri(baseURI, String.Format(TOPIC_PATH, Topic.GenerateUrn(core, topicId)));
            
            WebRequest request = WebRequest.Create(completeURI);
            request = PrepareRequest(request, core, null);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            // throws on >= 400
            evaluateResponse(response);

            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            string responseString = responseReader.ReadToEnd();

            responseReader.Close();
            responseStream.Close();

            // is this satisfied?
            //return Topic.SerializeFromJson(
              //      content.getAsJsonObject("data").getAsJsonObject("topic"));
            // possibly convert to JObject first and pluck out data attr into New Topic
            return JsonConvert.DeserializeObject<Topic>(responseString);

        }

        
        public static Topic CreateOrUpdateTopic(LFCore core, string topicId, string label) {
            Topic t = Topic.Create(core, topicId, label);

            List<Topic> topics = new List<Topic>();
            topics.Add(t);

            return CreateOrUpdateTopics(core, topics).ElementAt(0);
        }
    

        public static bool deleteTopic(LFCore core, Topic topic) {
            List<Topic> topics = new List<Topic>();

            topics.Add(topic);

            return DeleteTopics(core, topics) == 1;
        }


        /* Multiple Topic API */
        public static List<Topic> GetTopics(LFCore core, int limit, int offset) {
            Uri baseURI = BuildURL(core);
            Uri wholeURI = new Uri(baseURI, String.Format(MULTIPLE_TOPIC_PATH, core.GetUrn()));

            // add params to uri
            // should check the right edge of the AbsoluteUri
            Uri limitParamURI = new Uri(wholeURI, String.Format("?limit={0}", limit.ToString() == null ? "100" : limit.ToString()));

            Uri completeURI = new Uri(limitParamURI, 
                String.Format("&offset={0}", offset.ToString() == null ? "0" : offset.ToString()));

            WebRequest request = WebRequest.Create(completeURI);
            request = PrepareRequest(request, core, null);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            // throws on >= 400
            evaluateResponse(response);

            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            string responseString = responseReader.ReadToEnd();

            responseReader.Close();
            responseStream.Close();

            return JsonConvert.DeserializeObject<List<Topic>>(responseString);

        }

    
        public static List<Topic> CreateOrUpdateTopics(LFCore core, List<Topic> topics) {
            topics.ForEach(delegate(Topic t) {
                TopicValidator.ValidateTopicLabel(t.GetLabel());
            });

            Uri baseURI = BuildURL(core);
            Uri completeURI = new Uri(baseURI, String.Format(MULTIPLE_TOPIC_PATH, core.GetUrn()));

            WebRequest request = WebRequest.Create(completeURI);
            request = PrepareRequest(request, core, null);
            request.Method = "POST";

            string jsonPostData = JsonConvert.SerializeObject(topics);
            // ascii or utf8?
            // byte[] postBytes = Encoding.ASCII.GetBytes(postData);
            byte[] postBytes = Encoding.UTF8.GetBytes(jsonPostData);
    
            // inject Post Data
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);

            requestStream.Close();


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            // throws on >= 400
            evaluateResponse(response);

            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            string responseString = responseReader.ReadToEnd();

            responseReader.Close();
            responseStream.Close();

            // returning response instead of pre-request/updated/created Topics
            return JsonConvert.DeserializeObject<List<Topic>>(responseString);
        }

    
        public static int DeleteTopics(LFCore core, List<Topic> topics) {
            topics.ForEach(delegate(Topic t) {
                TopicValidator.ValidateTopicLabel(t.GetLabel());
            });

            Uri baseURI = BuildURL(core);
            Uri wholeURI = new Uri(baseURI, String.Format(MULTIPLE_TOPIC_PATH, core.GetUrn()));
            // Insert Patch Method
            // STRING!  config me!
            Uri completeURI = new Uri(wholeURI, String.Format("&_method={0}", PATCH_METHOD));
            

            WebRequest request = WebRequest.Create(completeURI);
            request = PrepareRequest(request, core, null);
            request.Method = "POST";

            string jsonPostData = JsonConvert.SerializeObject(topics);
            // ascii or utf8?
            // byte[] postBytes = Encoding.ASCII.GetBytes(postData);
            byte[] postBytes = Encoding.UTF8.GetBytes(jsonPostData);
    
            // inject Post Data
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);

            requestStream.Close();


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            // throws on >= 400
            evaluateResponse(response);

            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            string responseString = responseReader.ReadToEnd();

            responseReader.Close();
            responseStream.Close();

            // return data.has("deleted") ? data.get("deleted").getAsInt() : 0;
            
            // JSON nested underneath data prop! - CHECK THIS EVERYWHERE

            JObject jsonResponse = JObject.Parse(responseString);
            
            // this may not work
            
            // Configurable String!
            // CHECK ME FOR CORRECT JSON PROP TREE
            int deleted = (int)jsonResponse.SelectToken("data.deleted");
            return deleted;
        }
    

        /* Collection Topic API */
        public static List<String> GetCollectionTopics(Collection collection) {
            Uri URI = BuildURL(collection);
            Uri completeURI = new Uri(URI, String.Format(MULTIPLE_TOPIC_PATH, collection.GetUrn()));

            WebRequest request = WebRequest.Create(completeURI);
            request = PrepareRequest(request, collection, null);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            // throws on >= 400
            evaluateResponse(response);

            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            string responseString = responseReader.ReadToEnd();

            responseReader.Close();
            responseStream.Close();

            JObject jsonResponse = JObject.Parse(responseString);

            JArray idArray = (JArray)jsonResponse["data"]["topicIds"];
            List<string> topicIDs = idArray.Select(id => (string)id).ToList();

            return topicIDs;

        }


        public static int AddCollectionTopics(Collection collection, List<Topic> topics) {
            List<string> topicIDs = new List<string>();

            topics.ForEach(delegate(Topic t) {
                //throws
                TopicValidator.ValidateTopicLabel(t.GetLabel());
                topicIDs.Add(t.GetId());
            });

            Dictionary<String, List<String>> idMap = new Dictionary<string, List<string>>();

            idMap.Add("topicIds", topicIDs);


            Uri baseURL  = BuildURL(collection);
            Uri wholeURI = new Uri(baseURL, String.Format(MULTIPLE_TOPIC_PATH, collection.GetUrn()));
            // STRING!  config me!
            Uri completeURI = new Uri(wholeURI, String.Format("&_method={0}", PATCH_METHOD));


            WebRequest request = WebRequest.Create(completeURI);
            request = PrepareRequest(request, collection, null);
            request.Method = "POST";


            string jsonPostData = JsonConvert.SerializeObject(idMap);
            // ascii or utf8?
            // byte[] postBytes = Encoding.ASCII.GetBytes(postData);
            byte[] postBytes = Encoding.UTF8.GetBytes(jsonPostData);


            // inject Post Data
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);

            requestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            // throws on >= 400
            evaluateResponse(response);

            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            string responseString = responseReader.ReadToEnd();

            responseReader.Close();
            responseStream.Close();

            JObject jsonResponse = JObject.Parse(responseString);

            // Confligg-able STRING!
            // CHECK THIS PROP TREE!
            JValue jvAdded = new JValue(jsonResponse["data"]["added"]);

            return (int)jvAdded > 0 ? (int)jvAdded : 0;
        }
     
    
        public static Dictionary<string, int> ReplaceCollectionTopics(Collection collection, List<Topic> topics) {
            // string form = LivefyreUtil.mapToJsonString(ImmutableMap.<String, Object>of("topicIds", getTopicIds(topics)));

            List<string> topicIDs = new List<string>();

            topics.ForEach(delegate(Topic t)
            {
                //throws
                TopicValidator.ValidateTopicLabel(t.GetLabel());
                topicIDs.Add(t.GetId());
            });

            Dictionary<String, List<String>> idMap = new Dictionary<string, List<string>>();

            idMap.Add("topicIds", topicIDs);

            Uri baseURI = BuildURL(collection);
            Uri completeURI = new Uri(baseURI, String.Format(MULTIPLE_TOPIC_PATH, collection.GetUrn()));

            WebRequest request = WebRequest.Create(completeURI);
            request = PrepareRequest(request, collection, null);
            request.Method = "PUT";

            string jsonPostData = JsonConvert.SerializeObject(idMap);
            // ascii or utf8?
            // byte[] postBytes = Encoding.ASCII.GetBytes(postData);
            byte[] postBytes = Encoding.UTF8.GetBytes(jsonPostData);
    
            // inject Post Data
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);

            requestStream.Close();


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            // throws on >= 400
            evaluateResponse(response);

            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            string responseString = responseReader.ReadToEnd();

            responseReader.Close();
            responseStream.Close();


            JObject jsonResponse = JObject.Parse(responseString);

            // Confligg-able STRING!
            // CHECK THIS PROP TREE!
            JValue jvAdded = new JValue(jsonResponse["data"]["added"]);
            JValue jvRemoved = new JValue(jsonResponse["data"]["removed"]);

            // EITHER USE lower type OR PROPER TYPE  CHOOSE ONE
            Dictionary<string, int> results = new Dictionary<string, int>();

            // more String cofigs
            results.Add("added", (int)jvAdded);
            results.Add("removed", (int)jvRemoved);


            return results;
        }


    /*
        public static int removeCollectionTopics(Collection collection, List<Topic> topics) {
            string form = LivefyreUtil.mapToJsonString(ImmutableMap.<String, Object>of("delete", getTopicIds(topics)));
        
            ClientResponse response = builder(collection)
                    .path(String.Format(MULTIPLE_TOPIC_PATH, collection.getUrn()))
                    .queryParam("_method", PATCH_METHOD)
                    .accept(MediaType.APPLICATION_JSON)
                    .type(MediaType.APPLICATION_JSON)
                    .post(ClientResponse.class, form);
            JsonObject content = evaluateResponse(response);
            JsonObject data = content.getAsJsonObject("data");

            return data.has("removed") ? data.get("removed").getAsInt() : 0;
        }
    */
        /* Subscription API */
/*
        public static List<Subscription> getSubscriptions(Network network, string userId) {
            ClientResponse response = builder(network)
                    .path(String.Format(USER_SUBSCRIPTION_PATH, network.getUrnForUser(userId)))
                    .accept(MediaType.APPLICATION_JSON)
                    .get(ClientResponse.class);
            JsonObject content = evaluateResponse(response);
            JsonArray subscriptionData = content.getAsJsonObject("data").getAsJsonArray("subscriptions");
        
            List<Subscription> subscriptions = Lists.newArrayList();
            if (subscriptionData != null) {
                for (int i = 0; i < subscriptionData.size(); i++) {
                    subscriptions.add(Subscription.serializeFromJson(subscriptionData.get(i).getAsJsonObject()));
                }
            }
            return subscriptions;
        }
    
        public static int addSubscriptions(Network network, string userToken, List<Topic> topics) {
            string userId = getUserFromToken(network, userToken);
            string userUrn = network.getUrnForUser(userId);
            string form = LivefyreUtil.mapToJsonString(ImmutableMap.<String, Object>of("subscriptions", buildSubscriptions(topics, userUrn)));

            ClientResponse response = builder(network, userToken)
                    .path(String.Format(USER_SUBSCRIPTION_PATH, userUrn))
                    .accept(MediaType.APPLICATION_JSON)
                    .type(MediaType.APPLICATION_JSON)
                    .post(ClientResponse.class, form);
            JsonObject content = evaluateResponse(response);
            JsonObject data = content.getAsJsonObject("data");

            return data.has("added") ? data.get("added").getAsInt() : 0;
        }
    
        public static Map<String, Integer> replaceSubscriptions(Network network, string userToken, List<Topic> topics) {
            string userId = getUserFromToken(network, userToken);
            string userUrn = network.getUrnForUser(userId);
            string form = LivefyreUtil.mapToJsonString(ImmutableMap.<String, Object>of("subscriptions", buildSubscriptions(topics, userUrn)));

            ClientResponse response = builder(network, userToken)
                    .path(String.Format(USER_SUBSCRIPTION_PATH, userUrn))
                    .accept(MediaType.APPLICATION_JSON)
                    .type(MediaType.APPLICATION_JSON)
                    .put(ClientResponse.class, form);
            JsonObject content = evaluateResponse(response);
            JsonObject data = content.getAsJsonObject("data");
        
            Map<String, Integer> results = Maps.newHashMap();
            results.put("added", data.has("added") ? data.get("added").getAsInt() : 0);
            results.put("removed", data.has("removed") ? data.get("removed").getAsInt() : 0);
            return results;
        }

        public static int removeSubscriptions(Network network, string userToken, List<Topic> topics) {
            string userId = getUserFromToken(network, userToken);
            string userUrn = network.getUrnForUser(userId);
            string form = LivefyreUtil.mapToJsonString(ImmutableMap.<String, Object>of("delete", buildSubscriptions(topics, userUrn)));

            ClientResponse response = builder(network, userToken)
                    .path(String.Format(USER_SUBSCRIPTION_PATH, userUrn))
                    .queryParam("_method", PATCH_METHOD)
                    .accept(MediaType.APPLICATION_JSON)
                    .type(MediaType.APPLICATION_JSON)
                    .post(ClientResponse.class, form);
            JsonObject content = evaluateResponse(response);
            JsonObject data = content.getAsJsonObject("data");

            return data.has("removed") ? data.get("removed").getAsInt() : 0;
        }
    
        public static List<Subscription> getSubscribers(Network network, Topic topic, int limit, int offset) {
            ClientResponse response = builder(network)
                    .path(String.Format(TOPIC_SUBSCRIPTION_PATH, topic.getId()))
                    .queryParam("limit", limit == null ? "100" : limit.toString())
                    .queryParam("offset", offset == null ? "0" : offset.toString())
                    .accept(MediaType.APPLICATION_JSON)
                    .get(ClientResponse.class);
            JsonObject content = evaluateResponse(response);
            JsonArray data = content.getAsJsonObject("data").getAsJsonArray("subscriptions");
        
            List<Subscription> subscriptions = Lists.newArrayList();
            if (data != null) {
                for (int i = 0; i < data.size(); i++) {
                    subscriptions.add(Subscription.serializeFromJson(data.get(i).getAsJsonObject()));
                }
            }
            return subscriptions;
        }
    */
        /* This call is used specifically by the TimelineCursor class. */
/*
        public static JsonObject getTimelineStream(TimelineCursor cursor, boolean isNext) {
            WebRequest r = streamBuilder(cursor.getCore())
                    .path(TIMELINE_PATH)
                    .queryParam("limit", cursor.getData().getLimit().toString())
                    .queryParam("resource", cursor.getData().getResource());
        
            if (isNext) {
                r = r.queryParam("since", cursor.getData().getCursorTime());
            } else {
                r = r.queryParam("until", cursor.getData().getCursorTime());
            }
        
            ClientResponse response = r.accept(MediaType.APPLICATION_JSON).get(ClientResponse.class);
            return evaluateResponse(response);
        }
  */  


        /* Helper methods */
        // builder becomes BuildURL
        // string should be Uri Type?
        // these two builders append uri information onto the current url
        private static Uri BuildURL(LFCore core)
        {
            //url should be checked for query? 
              //  didnt Lookup like any params, but CHECK!
            // can also be:
            return new Uri(String.Format(BASE_URL, Domain.quill(core)));
            // awful, temporary
            // return url += String.Format(BASE_URL, Domain.quill(core));

        }

        private static Uri BuildStreamURL(LFCore core)
        {
            return new Uri(String.Format(BASE_URL, Domain.bootstrap(core)));
            // awful, temporary
            // return url += String.Format(STREAM_BASE_URL, Domain.bootstrap(core));
        }

        private static WebRequest PrepareRequest(WebRequest request, LFCore core, string userToken)
        {
            // more configurable/member-able dataz
            request.ContentType = "application/json";
            // BEWARE of this SETTING for all requests
            request.Headers.Set("Accept", "application/json");
            // PATCH is added as _method PARAM in individual calls!
            // request.Method = PATCH_METHOD;
            // is request.Timeout connect, read or both in Java Jersey Client terms?
            // ClientConfig.PROPERTY_CONNECT_TIMEOUT, 1000
            // ClientConfig.PROPERTY_READ_TIMEOUT, 10000
            // this timeout should be confliggle-able?
            // request.Timeout = something reasonable/tested
            request.Timeout = 10000;

            // USER AGENT MAY BE NECESSARY! 
            //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";

            LftokenAuthFilter tokenFilter = new LftokenAuthFilter(core, userToken);

            tokenFilter.AddAuthHeader(request);

            return request;
        }

        // evaluates && casts - separate?
        // dropping the cast
        private static void evaluateResponse(HttpWebResponse response) {
            // responseCode check - make Utils method
            // anything greater than 200 would break?
            // should be 200 or ERROR?
            // if ((int)response.StatusCode !== 200) {
            if ((int)response.StatusCode >= 400)
            {
                //throw new ApiException(response.GetStatus());
                throw new Exception(String.Format("An error has occured: {0}", response.StatusCode));
            }

        }
    
        
        private static List<String> GetTopicIds(List<Topic> topics) {
            List<String> ids = new List<string>();

            topics.ForEach(delegate(Topic t) {
                ids.Add(t.GetId());
            });

            return ids;
        }

    
        private static List<Subscription> BuildSubscriptions(List<Topic> topics, string userUrn) {
            List<Subscription> subscriptions = new List<Subscription>();

            topics.ForEach(delegate(Topic t){
                // pass 0 here so that the server can create the timestamp or gen UnixNow?
                subscriptions.Add(new Subscription(t.GetId(), userUrn, SubscriptionType.PersonalStream, 0));
            });

            return subscriptions;
        }


        /*
         * JWT HERE
        private static string getUserFromToken(Network network, string userToken) {
            JsonObject json = LivefyreUtil.decodeJwt(userToken, network.getData().getKey());
            return json.get("user_id").getAsString();
        }

        */



    }

}
