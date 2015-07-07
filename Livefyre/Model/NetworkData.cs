using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Model
{
    public class NetworkData
    {
        private string name = null;
        private string key = null;

        public NetworkData(string name, string key)
        {
            this.name = name;
            this.key = key;
        }

        public string GetName()
        {
            return name;
        }

        public NetworkData SetName(string name)
        {
            this.name = name;
            return this;
        }

        public string GetKey()
        {
            return key;
        }

        public NetworkData SetKey(string key)
        {
            this.key = key;
            return this;
        }
    }
}
