﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Core;

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
        public Topic() { }
    
        public Topic(string ID , string Label, int CreatedAt, int ModifiedAt) {
            this.ID  = ID ;
            this.Label = Label;
            this.CreatedAt = CreatedAt;
            this.ModifiedAt = ModifiedAt;
        }
        
        // why not pull this into constructor?
        /* Use this method to generate Topic objects. Otherwise ID 's (urns) will not be formed properly. */
        public static Topic create(LFCore core, string ID , string Label) {
            return new Topic(GenerateUrn(core, ID), Label, null, null);
        }
    
        public static string GenerateUrn(LFCore core, string ID ) {
            return core.GetUrn() + TOPIC_IDENTIFIER + ID ;
        }

        public static Topic serializeFromJson(JsonObject json) {
            return new Topic(
                json.get("ID").getAsString(),
                json.get("Label").getAsString(),
                json.get("CreatedAt").getAsInt(),
                json.get("ModifiedAt").getAsInt());
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

        public int getCreatedAt() {
            return CreatedAt;
        }

        public void  setCreatedAt(int CreatedAt) {
            this.CreatedAt = CreatedAt;
        }

        public int getModifiedAt() {
            return ModifiedAt;
        }

        public void setModifiedAt(int ModifiedAt) {
            this.ModifiedAt = ModifiedAt;
        }

    }
}
