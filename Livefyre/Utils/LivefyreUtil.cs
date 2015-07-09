using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using Livefyre.Core;

using JWT;

using Newtonsoft.Json;



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


        public static string SerializeAndSign (Dictionary<string, object> claims, string key) {
            // too easy?!
            try
            {
                return JWT.JsonWebToken.Encode(claims, key, JWT.JwtHashAlgorithm.HS256);
            }
            catch (Exception)
            {
                // shorthand works?
                throw;
            }
        }


        public static string DecodeJWT(string jwt, string key) {
            try
            {
                return JWT.JsonWebToken.Decode(jwt, key);
            }
            catch (JWT.SignatureVerificationException)
            {
                throw;
            }
        }


        public static Int32 UnixNow()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

    }
}
