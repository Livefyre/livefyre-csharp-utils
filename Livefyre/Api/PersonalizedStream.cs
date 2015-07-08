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
    


        /* Topic API */

        public static Topic GetTopic(LFCore core, string topicId) {
            Uri uri = new Uri(String.Format(TOPIC_PATH, Topic.GenerateUrn(core, topicId)));
            
            Uri completeURI = BuildURL(uri, core);
            WebRequest request = WebRequest.Create(uri);

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
            return JsonConvert.DeserializeObject<Topic>(responseString);

        }

        /*
        public static Topic createOrUpdateTopic(LFCore core, string topicId, string label) {
            return createOrUpdateTopics(core, ImmutableMap.of(topicId, label)).get(0);
        }
    
        public static boolean deleteTopic(LFCore core, Topic topic) {
            return deleteTopics(core, Lists.newArrayList(topic)) == 1;
        }
         * 
         */

        /* Multiple Topic API */
        public static List<Topic> GetTopics(LFCore core, int limit, int offset) {
            Uri uri = new Uri(String.Format(MULTIPLE_TOPIC_PATH, core.GetUrn()));
            uri = BuildURL(uri, core);

            // add params to uri
            // should check the right edge of the AbsoluteUri
            Uri limitParam = new Uri(uri, String.Format("?limit={0}", limit.ToString() == null ? "100" : limit.ToString()));
            Uri completeURL = new Uri(uri, String.Format("&offset={0}", offset.ToString() == null ? "0" : offset.ToString()));

            WebRequest request = WebRequest.Create(completeURL);
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

    
        public static List<Topic> createOrUpdateTopics(LFCore core, List<Topic> topics) {
            topics.ForEach(delegate(Topic t) {
                TopicValidator.ValidateTopicLabel(t.GetLabel());
            });

            Uri baseURL = new Uri(String.Format(MULTIPLE_TOPIC_PATH, core.GetUrn()));
            Uri completeURL = BuildURL(baseURL, core);

            WebRequest request = WebRequest.Create(completeURL);
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

    /*
    
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
        private static Uri BuildURL(Uri url, LFCore core)
        {
            //url should be checked for query? 
              //  didnt Lookup like any params, but CHECK!
            // can also be:
            return new Uri(url, String.Format(BASE_URL, Domain.quill(core)));
            // awful, temporary
            // return url += String.Format(BASE_URL, Domain.quill(core));

        }

        private static Uri BuildStreamURL(Uri url, LFCore core)
        {
            return new Uri(url, String.Format(BASE_URL, Domain.quill(core)));
            // awful, temporary
            // return url += String.Format(STREAM_BASE_URL, Domain.bootstrap(core));
        }

        private static WebRequest PrepareRequest(WebRequest request, LFCore core, string userToken)
        {
            request.ContentType = "application/json";
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
