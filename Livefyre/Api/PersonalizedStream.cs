using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Livefyre.Api.Filter;
using Livefyre.Core;
using Livefyre.Dto;
using Livefyre.Utils;

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
    


        /* Topic API */

        public static Topic GetTopic(LFCore core, string topicId) {

            Uri uri = new Uri(String.Format(TOPIC_PATH, Topic.GenerateUrn(core, topicId)));

            WebRequest request = WebRequest.Create(uri);
            
            request = builder(request, core);

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
            return JsonConvert.DeserializeObject<Topic>(responseString);
        }
    /*
        public static Topic createOrUpdateTopic(LFCore core, string topicId, string label) {
            return createOrUpdateTopics(core, ImmutableMap.of(topicId, label)).get(0);
        }
    
        public static boolean deleteTopic(LFCore core, Topic topic) {
            return deleteTopics(core, Lists.newArrayList(topic)) == 1;
        }
    */
        /* Multiple Topic API */
    /*
        public static List<Topic> getTopics(LFCore core, Integer limit, Integer offset) {
            ClientResponse response = builder(core)
                    .path(String.Format(MULTIPLE_TOPIC_PATH, core.getUrn()))
                    .queryParam("limit", limit == null ? "100" : limit.toString())
                    .queryParam("offset", offset == null ? "0" : offset.toString())
                    .accept(MediaType.APPLICATION_JSON)
                    .get(ClientResponse.class);
            JsonObject content = evaluateResponse(response);
            JsonArray topicsData = content.getAsJsonObject("data").getAsJsonArray("topics");
        
            List<Topic> topics = Lists.newArrayList();
            if (topicsData != null) {
                for (int i = 0; i < topicsData.size(); i++) {
                    topics.add(Topic.serializeFromJson(topicsData.get(i).getAsJsonObject()));
                }
            }
            return topics;
        }
    
        public static List<Topic> createOrUpdateTopics(LFCore core, Map<String, String> topicMap) {
            List<Topic> topics = Lists.newArrayList();
            for (String k : topicMap.keySet()) {
                string label = topicMap.get(k);
            
                if (StringUtils.isEmpty(label) || label.length() > 128) {
                    throw new IllegalArgumentException("Topic label is of incorrect length or empty.");
                }
            
                topics.add(Topic.create(core, k, label));
            }
            string form = LivefyreUtil.mapToJsonString(ImmutableMap.<String, Object>of("topics", topics));
        
            ClientResponse response = builder(core)
                    .path(String.Format(MULTIPLE_TOPIC_PATH, core.getUrn()))
                    .accept(MediaType.APPLICATION_JSON)
                    .type(MediaType.APPLICATION_JSON)
                    .post(ClientResponse.class, form);
            evaluateResponse(response);
        
            // Doesn't matter what the response details are here as long as it's a 200.
            return topics;
        }
    
        public static int deleteTopics(LFCore core, List<Topic> topics) {
            string form = LivefyreUtil.mapToJsonString(ImmutableMap.<String, Object>of("delete", getTopicIds(topics)));
        
            ClientResponse response = builder(core)
                    .path(String.Format(MULTIPLE_TOPIC_PATH, core.getUrn()))
                    .queryParam("_method", PATCH_METHOD)
                    .accept(MediaType.APPLICATION_JSON)
                    .type(MediaType.APPLICATION_JSON)
                    .post(ClientResponse.class, form);
            JsonObject content = evaluateResponse(response);
            JsonObject data = content.getAsJsonObject("data");
        
            return data.has("deleted") ? data.get("deleted").getAsInt() : 0;
        }
  */  
        /* Collection Topic API */
/*
        public static List<String> getCollectionTopics(Collection collection) {
            ClientResponse response = builder(collection)
                    .path(String.Format(MULTIPLE_TOPIC_PATH, collection.getUrn()))
                    .accept(MediaType.APPLICATION_JSON)
                    .get(ClientResponse.class);
            JsonObject content = evaluateResponse(response);
            JsonArray topicData = content.getAsJsonObject("data").getAsJsonArray("topicIds");
        
            List<String> topicIds = Lists.newArrayList();
            if (topicData != null) {
                for (int i = 0; i < topicData.size(); i++) {
                    topicIds.add(topicData.get(i).getAsString());
                }
            }
            return topicIds;
        }
    
        public static int addCollectionTopics(Collection collection, List<Topic> topics) {
            string form = LivefyreUtil.mapToJsonString(ImmutableMap.<String, Object>of("topicIds", getTopicIds(topics)));
        
            ClientResponse response = builder(collection)
                    .path(String.Format(MULTIPLE_TOPIC_PATH, collection.getUrn()))
                    .accept(MediaType.APPLICATION_JSON)
                    .type(MediaType.APPLICATION_JSON)
                    .post(ClientResponse.class, form);
            JsonObject content = evaluateResponse(response);
            JsonObject data = content.getAsJsonObject("data");
    
            return data.has("added") ? data.get("added").getAsInt() : 0;
        }
    
        public static Map<String, Integer> replaceCollectionTopics(Collection collection, List<Topic> topics) {
            string form = LivefyreUtil.mapToJsonString(ImmutableMap.<String, Object>of("topicIds", getTopicIds(topics)));

            ClientResponse response = builder(collection)
                    .path(String.Format(MULTIPLE_TOPIC_PATH, collection.getUrn()))
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
    
        public static List<Subscription> getSubscribers(Network network, Topic topic, Integer limit, Integer offset) {
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

        private static WebRequest builder(WebRequest request, LFCore core) {
            return builder(request, core, null);
        }
    
        // these two builders append uri information onto the current url path
        private static WebRequest builder(WebRequest request, LFCore core, string userToken) {
            // builder(core, userToken)
            //    .resource(String.Format(BASE_URL, Domain.quill(core)));

            // return WebRequest with appended Path info
            return request;
        }
    
        private static WebRequest streamBuilder(WebRequest request, LFCore core) {
         //       builder(core, null).resource(String.Format(STREAM_BASE_URL, Domain.bootstrap(core)));
            return request;
        }

        private static WebRequest client(WebRequest request, LFCore core, string userToken) {

            //WebRequest request = WebRequest.Create(); 
            //= WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = PATCH_METHOD;
            // is this connect, read or both in Java Jersey client terms?
            // ClientConfig.PROPERTY_CONNECT_TIMEOUT, 1000
            // ClientConfig.PROPERTY_READ_TIMEOUT, 10000
            // request.Timeout = something reasonable/tested

            //LftokenAuthFilter tokenFilter = new LftokenAuthFilter(core, );

            // generate 
            request.Headers.Set("Authorization","");

            // USER AGENT MAY BE NECESSARY!
            //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";

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
                // make LF Exception
                //throw new ApiException(response.GetStatus());
                throw new Exception(String.Format("An error has occured: {0}", response.StatusCode));
            }

        }
    
        /*
        private static List<String> getTopicIds(List<Topic> topics) {
            List<String> ids = Lists.newArrayList();
            for (Topic topic : topics) {
                ids.add(topic.getId());
            }
            return ids;
        }
    
        private static List<Subscription> buildSubscriptions(List<Topic> topics, string userUrn) {
            List<Subscription> subscriptions = Lists.newArrayList();
            for (Topic topic : topics) {
                subscriptions.add(new Subscription(topic.getId(), userUrn, SubscriptionType.personalStream, null));
            }
            return subscriptions;
        }

        private static string getUserFromToken(Network network, string userToken) {
            JsonObject json = LivefyreUtil.decodeJwt(userToken, network.getData().getKey());
            return json.get("user_id").getAsString();
        }



        */







    }

}
