using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Core;
using Livefyre.Cursor;
using Livefyre.Dto;


namespace Livefyre.Factory
{
    public class CursorFactory
    {
        public static TimelineCursor GetTopicStreamCursor(LFCore core, Topic topic)
        {
            // UTC or Local Time?!
            return GetTopicStreamCursor(core, topic, 50, DateTime.Now);
        }

        public static TimelineCursor GetTopicStreamCursor(LFCore core, Topic topic, int limit, DateTime date)
        {
            // configzy string
            string resource = topic.GetId() + ":topicStream";
            return TimelineCursor.Init(core, resource, limit, date);
        }

        public static TimelineCursor GetPersonalStreamCursor(Network network, string user)
        {
            // UTC or Local Time?!
            return GetPersonalStreamCursor(network, user, 50, DateTime.Now);
        }


        public static TimelineCursor GetPersonalStreamCursor(Network network, string userId, int limit, DateTime date)
        {
            string resource = network.GetUrnForUser(userId) + ":personalStream";
            return TimelineCursor.Init(network, resource, limit, date);
        }
    }
}
