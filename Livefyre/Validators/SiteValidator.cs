using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livefyre.Model;


namespace Livefyre.Validators
{
    public class SiteValidator : Validator<SiteData> {

        public SiteData Validate(SiteData data) {

            StringBuilder reason = new StringBuilder();

            if (String.IsNullOrEmpty(data.GetId())) {
                reason.Append("\n ID is null or blank.");
            }

            if (String.IsNullOrEmpty(data.GetKey()))
            {
                reason.Append("\n Key is null or blank.");
            }

            if (reason.Length > 0)
            {
                throw new Exception(String.Format("Problems with your site input: \n {0} \n", reason.ToString()));
            }

            return data;
        }

    }
}
