using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Type
{

    // THIS MAY BE A MESS OR MAY WORK...

    public class CollectionType
    {

        public static readonly string COUNTING = "counting";
        public static readonly string BLOG = "blog";
        public static readonly string CHAT = "chat";
        public static readonly string COMMENTS = "comments";
        public static readonly string RATINGS = "ratings";
        public static readonly string SIDENOTES = "sidenotes";

        //make sense of this from the JAVA Enum side of things
        private string type;
    
        private CollectionType(string type) {
            this.type = type;
        }
    
        public string toString() {
            return type;
        }

    }
}
