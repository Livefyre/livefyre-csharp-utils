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
        public string next()
        {
            //var responseData = PersonalizedStream.

            return null;
        }

        /**
         * Returns the previous set of events in the timeline from the cursorTime.

         * @return JSONObject
         */

        // JObject here?

        public string previous()
        {
            return null;
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
