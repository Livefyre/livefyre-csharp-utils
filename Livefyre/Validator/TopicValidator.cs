using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Dto;


namespace Livefyre.Validator
{
    class TopicValidator : Validator
    {
        public string Validate(Topic t)
        {
            

            // checking topic key length
            // make part of validation
            /*
            string label = topicMap.get(k);
            if (StringUtils.isEmpty(label) || label.length() > 128)
            {
                throw new IllegalArgumentException("Topic label is of incorrect length or empty.");
            }
             */
            // fill me in
            return null;
        }

        public static void ValidateTopicKey(string key)
        {
            if (key == null || key.Length == 0 )
            {
                throw new ArgumentOutOfRangeException("key", "The Topic Key appears to be empty");

            }
            else if (key.Length > 128)
            {
                throw new ArgumentOutOfRangeException("key", "The Topic Key appears to be greater than 128 chars");

            }
        }

    }
}
