using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Dto;


namespace Livefyre.Validators
{
    public class TopicValidator : Validator<Topic>
    {

        public string Validate(Topic t)
        {
            Console.WriteLine(t);
            return null;
        }

        public static void ValidateTopicLabel(string label)
        {
            // checking topic key length

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
