using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Model
{
    class SiteData
    {
        private string id;
        private string key;

        public SiteData(string id, string key)
        {
            this.id = id;
            this.key = key;
        }

        public string GetId()
        {
            return id;
        }

        public SiteData SetId(string id)
        {
            this.id = id;
            return this;
        }

        public string GetKey()
        {
            return key;
        }

        public SiteData SetKey(string key)
        {
            this.key = key;
            return this;
        }

    }
}
