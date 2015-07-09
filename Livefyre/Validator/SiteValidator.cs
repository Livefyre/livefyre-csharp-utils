using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Model;


namespace Livefyre.Validator
{
    public class SiteValidator : Validator {

        public string validate(SiteData data) {

            StringBuilder reason = new StringBuilder();

            if (String.IsNullOrEmpty(data.GetId())) {
                reason.Append("\n ID is null or blank.");
            }

            if (String.IsNullOrEmpty(data.GetKey()))
            {
                reason.Append("\n Key is null or blank.");
            }

            if (reason.Length > 0) {
                 return String.Format("Problems with your site input: {0}", reason);
            }

            return null;
        }

    }
}
