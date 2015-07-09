using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Api;
using Livefyre.Core;
using Livefyre.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Livefyre.Cursor
{

    /**
     * TimelineCursor is an entity that keeps track of positions in timelines for different resources.
     * When created via a Network object, it will keep track of timelines at the network level, and
     * likewise at the site level if created via a Site object.
     */
    public class TimelineCursor
    {
        private LFCore core;
        private CursorData data;

        public TimelineCursor(LFCore core, CursorData data)
        {
            this.core = core;
            this.data = data;
        }

        public static TimelineCursor Init(LFCore core, String resource, int limit, DateTime startTime)
        {
            CursorData data = new CursorData(resource, limit, startTime);

            //return new TimelineCursor(core, ReflectiveValidator.validate(data));
            // validate me
            return new TimelineCursor(core, data);
        }

        /**
         * Returns the next set of events in the timeline from the cursorTime.

         * @return JSONObject
         */
        // JObject here?
        // no native Type - return json string for consumers 
        // public JObject next()
        public string Next()
        {
            string responseString = PersonalizedStream.GetTimelineStream(this, false);

            //Cursor cursor = JsonConvert.DeserializeObject<Cursor>(responseString);

            JObject jsonResponse = JObject.Parse(responseString);

            // Confligg-able STRING!
            // CHECK THIS PROP TREE!
            bool hasNext = (bool)jsonResponse["meta"]["cursor"]["hasNext"];

            // might have to go from string to bool
            string nextDate = (string)jsonResponse["meta"]["cursor"]["next"];

            // urgh?
            bool next = nextDate.Length > 0 ? true : false;

            data.SetPrevious(hasNext);
            data.SetNext(next);

            if (data.isNext())
            {
                data.SetCursorTime(nextDate);
            }

            return responseString;

        }

        /**
         * Returns the previous set of events in the timeline from the cursorTime.

         * @return JSONObject
         */

        // JObject here?

        public string Previous()
        {
            string responseString = PersonalizedStream.GetTimelineStream(this, false);

            //Cursor cursor = JsonConvert.DeserializeObject<Cursor>(responseString);

            JObject jsonResponse = JObject.Parse(responseString);

            // Confligg-able STRING!
            // CHECK THIS PROP TREE!
            bool hasPrev = (bool)jsonResponse["meta"]["cursor"]["hasPrev"];

            // might have to go from string to bool
            string prevDate = (string)jsonResponse["meta"]["cursor"]["prev"];

            // urgh?
            bool prev = prevDate.Length > 0 ? true : false;

            data.SetPrevious(hasPrev);
            data.SetNext(prev);

            if (data.isNext())
            {
                data.SetCursorTime(prevDate);
            }

            return responseString;
        }


 

        public LFCore GetCore()
        {
            return core;
        }

        public void SetCore(LFCore core)
        {
            this.core = core;
        }

        public CursorData GetData()
        {
            return data;
        }

        public void SetData(CursorData data)
        {
            this.data = data;
        }
    }

}
