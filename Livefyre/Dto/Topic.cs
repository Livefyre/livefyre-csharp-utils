using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Livefyre.Core;
using Livefyre.Utils;

using Newtonsoft.Json;

namespace Livefyre.Dto
{

    class Topic
    {

        private static string TOPIC_IDENTIFIER = ":topic=";

        private string ID;

        private string Label;

        private int CreatedAt;

        private int ModifiedAt;


        // necessary for general type?
        public Topic() {}
    
            //why int?  from the unix millis?
        public Topic(string ID, string Label, int CreatedAt, int ModifiedAt) {
            this.ID  = ID ;
            this.Label = Label;
            this.CreatedAt = CreatedAt;
            this.ModifiedAt = ModifiedAt;
        }
        
        // why not pull this into constructor?
        /* Use this method to generate Topic objects. Otherwise ID 's (urns) will not be formed properly. */
        public static Topic create(LFCore core, string ID, string label) {
            string urn = GenerateUrn(core, ID);
            Int32 unixTS = LivefyreUtil.UnixNow();

            Topic topic = new Topic(urn, label, unixTS, unixTS);
            return topic;
        }
    
        public static string GenerateUrn(LFCore core, string ID) {
            return core.GetUrn() + TOPIC_IDENTIFIER + ID ;
        }


        public static Topic serializeFromJson(string json) {
            Topic topic = JsonConvert.DeserializeObject<Topic>(json);
            return topic;
        }
    

        public string TruncatedId() {
            return ID.Substring(ID.IndexOf(TOPIC_IDENTIFIER) + TOPIC_IDENTIFIER.Length);
        }

        public DateTime CreatedAtDate() {
            // UNIX Time
            DateTime utcStart = new DateTime(1970, 1, 1);
            return utcStart.AddSeconds(CreatedAt);
        }

        public DateTime ModifiedAtDate() {
            DateTime utcStart = new DateTime(1970, 1, 1);
            return utcStart.AddSeconds(ModifiedAt);
        }


        // make C# style
        /* Getters/Setters */
        public string GetId() {
            return ID ;
        }

        public void SetId(string ID ) {
            this.ID  = ID ;
        }

        public string GetLabel() {
            return Label;
        }

        public void SetLabel(string Label) {
            this.Label = Label;
        }

        public int GetCreatedAt() {
            return CreatedAt;
        }

        public void  SetCreatedAt(int CreatedAt) {
            this.CreatedAt = CreatedAt;
        }

        public int GetModifiedAt() {
            return ModifiedAt;
        }

        public void GetModifiedAt(int ModifiedAt) {
            this.ModifiedAt = ModifiedAt;
        }

    }
}
