using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Dto;


namespace Livefyre.Validator
{
    public class TopicValidator : Validator
    {

        public string Validate(System.Type t)
        {
            return null;
        }

        public static void ValidateTopicLabel(string label)
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

            if (label == null || label.Length == 0 )
            {
                throw new ArgumentOutOfRangeException("label", "The Topic Key appears to be empty");

            }
            else if (label.Length > 128)
            {
                throw new ArgumentOutOfRangeException("label", "The Topic Key appears to be greater than 128 chars");

            }
        }

    }
}
