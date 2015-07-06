using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using Livefyre.Core;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Livefyre.Utils
{
    public static class LivefyreUtil
    {
        // grab that JSON func

        // generic serializer here
        // unit test this, like real hard
        // in this context, more like, jsonString to marshalled Type
        // StringToJson
        public static T JsonToType<T>(string jsonData) {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer seri = new DataContractJsonSerializer(typeof(T));

            seri.WriteObject(stream, jsonData);
            
            T marshalledType = (T)seri.ReadObject(stream);

            return marshalledType;
        }

        //mapToJsonString
        public static string TypeToJson<T>(System.Type type) {
            string json;

            try {
                DataContractJsonSerializer seri = new DataContractJsonSerializer(typeof(T));
                MemoryStream stream = new MemoryStream();
                seri.WriteObject(stream, type);
                StreamReader reader = new StreamReader(stream);
                json = reader.ReadToEnd();
            } 
            catch (Exception e) {
                throw e;
            }

            return json;
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
