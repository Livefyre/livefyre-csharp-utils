using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Livefyre.Core;



namespace Livefyre.Utils
{
    public static class LivefyreUtil
    {

        public static T StringToJson<T>(string jsonData) {
            T typed = JsonConvert.DeserializeObject<T>(jsonData);
            return typed;
        }
        
        // mapToJsonString
        public static string TypeToJsonString (Object o) {
            string s = JsonConvert.SerializeObject(o);
            return s;
        }



        public static Network GetNetworkFromCore(LFCore core) {

            //class name
            string coreType = core.GetType().ToString();
            
            if (coreType == typeof(Network).ToString()) {
                return (Network)core;

            } else if (coreType == typeof(Site).ToString()) {
                return ((Site)core).GetNetwork();

            } else {
                return ((Collection)core).GetSite().GetNetwork();

            }

        }


        public static bool isValidFullUrl(String url) {
            try {
                Uri uri = new Uri(url);

            } catch (Exception) {
                return false;
            }

            return true;
        }

        /*
        // change Map param type
        public static String serializeAndSign(Map<String, Object> claims, String key) {
            // grab lib
            JsonWebSignature jws = new JsonWebSignature();
            jws.setPayload(new Gson().toJson(claims));
            jws.setAlgorithmHeaderValue(AlgorithmIdentifiers.HMAC_SHA256);
            jws.setHeader("typ", "JWT");
            jws.setKey(new HmacKey(Arrays.copyOf(key.getBytes(), 32)));

            try {
                return jws.getCompactSerialization();
            } catch (JoseException e) {
                throw new TokenException(e);
            }
        }
         * 
         */
    /*
        public static JsonObject decodeJwt(String jwt, String key) {
            JwtConsumer jwtConsumer;
            try {
                jwtConsumer = new JwtConsumerBuilder()
                        .setVerificationKey(new HmacKey(Arrays.copyOf(key.getBytes(), 32)))
                        .build();
                return new Gson().fromJson(jwtConsumer.processToClaims(jwt).toJson(), JsonObject.class);
            } catch (InvalidJwtException e) {
                throw new TokenException(e);
            }
        }
    */

        public static Int32 UnixNow()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

    }
}
