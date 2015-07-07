using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            SubscriptionType type;
            try
            {
                type = SubscriptionType.valueOf(json.get("type").getAsString());
            }
            catch (IllegalArgumentException e)
            {
                type = SubscriptionType.fromNum(json.get("type").getAsInt());
            }

            return new Subscription(
                json.get("to").getAsString(),
                json.get("by").getAsString(),
                type,
                json.get("createdAt").getAsInt());
        }

        public Date createdAtDate()
        {
            return new Date(createdAt.longValue() * 1000);
        }
        
        /* Getters/Setters */
        public String getTo()
        {
            return to;
        }

        public void setTo(String to)
        {
            this.to = to;
        }

        public String getBy()
        {
            return by;
        }

        public void setBy(String by)
        {
            this.by = by;
        }

        public String getType()
        {
            return type;
        }

        public void setType(String type)
        {
            this.type = type;
        }

        public Integer getCreatedAt()
        {
            return createdAt;
        }

        public void setCreatedAt(Integer createdAt)
        {
            this.createdAt = createdAt;
        }
    }

}
