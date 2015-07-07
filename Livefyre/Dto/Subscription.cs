using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Type;

using Newtonsoft.Json;

namespace Livefyre.Dto
{

    public class Subscription
    {
        private string to;
        private string by;
        private string type;
        private int createdAt;

        public Subscription() { }

        public Subscription(string to, string by, SubscriptionType type, int createdAt)
        {
            this.to = to;
            this.by = by;
            this.type = type.toString();
            this.createdAt = createdAt;
        }

        // haz json type?
        public static Subscription SerializeFromJson(string json)
        {
            // try/catch this?
            Subscription s = JsonConvert.DeserializeObject<Subscription>(json);

            return s;    
        }


        public DateTime createdAtDate()
        {
            //suspect!
            return new DateTime(createdAt * 1000);
        }
        
        /* Getters/Setters */
        public string getTo()
        {
            return to;
        }

        public void setTo(string to)
        {
            this.to = to;
        }

        public string getBy()
        {
            return by;
        }

        public void setBy(string by)
        {
            this.by = by;
        }

        public string getType()
        {
            return type;
        }

        public void setType(string type)
        {
            this.type = type;
        }

        public int getCreatedAt()
        {
            return createdAt;
        }

        public void setCreatedAt(int createdAt)
        {
            this.createdAt = createdAt;
        }
    }

}
