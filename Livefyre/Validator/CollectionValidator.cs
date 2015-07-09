using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Model;
using Livefyre.Utils;



namespace Livefyre.Validator
{
    public class CollectionValidator : Validator {

        public String Validate(CollectionData data) {

            StringBuilder reason = new StringBuilder();

            if (String.IsNullOrEmpty(data.GetArticleId())) {
                reason.Append("\n Article id is null or blank.");

            }

            if (String.IsNullOrEmpty(data.GetTitle())) {
                reason.Append("\n Title is null or blank.");

            } else if (data.GetTitle().Length > 255) {
                reason.Append("\n Title is longer than 255 characters.");

            }

            if (String.IsNullOrEmpty(data.GetUrl())) {
                reason.Append("\n URL is null or blank.");

            } else if (!LivefyreUtil.isValidFullUrl(data.GetUrl())) {
                reason.Append("\n URL is not a valid url. see http://www.ietf.org/rfc/rfc2396.txt");

            }

            if (data.GetType() == null) {
                reason.Append("\n Type is null.");

            }

            if (reason.Length > 0)
            {
                return String.Format("Problems with your collection input: {0}", reason.ToString());
            }

            return null;
        }
    }
}
