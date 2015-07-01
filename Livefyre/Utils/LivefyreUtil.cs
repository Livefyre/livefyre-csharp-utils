using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Core;

namespace Livefyre.Utils
{
    public static class LivefyreUtil
    {
        // grab that JSON func

        /*
        public static JsonObject stringToJson(String json) {
            Gson gson = new Gson();
            return gson.fromJson(json, JsonObject.class);
        }
    
        public static string mapToJsonString(Map<String, Object> map) {
            Gson gson = new Gson();
            return gson.toJson(map);
        }
        */
        public static Network GetNetworkFromCore(LFCore core) {
            /*
             this.GetType().Name

            if (core.getClass().equals(Network.class)) {
                return (Network) core;
            } else if (core.getClass().equals(Site.class)) {
                return ((Site) core).getNetwork();
            } else {
                return ((Collection) core).getSite().getNetwork();
            }
            */

            Network network = null;

            return network;
        }

        public static bool isValidFullUrl(String url) {
            try {
                Uri uri = new Uri(url);

            } catch (Exception e) {
                return false;
            }

            return true;
        }

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



    }
}
