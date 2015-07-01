using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Type
{
 
   // may not need extension methods

    public enum CollectionType {
        COUNTING = "counting",
        BLOG = "liveblog",
        CHAT = "livechat",
        COMMENTS = "livecomments",
        RATINGS = "ratings",
        REVIEWS = "reviews",
        SIDENOTES = "sidenotes"
    }

}
