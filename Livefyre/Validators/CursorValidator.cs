using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Livefyre.Model;

namespace Livefyre.Validators
{

    public class CursorValidator : Validator {

        public string Validate(CursorData data) {

            StringBuilder reason = new StringBuilder();

            if (String.IsNullOrEmpty(data.GetResource())) {
                reason.Append("\n Resource is null or blank.");

            }
        
            if (data.GetLimit() == null) {
                reason.Append("\n Limit is null.");

            }
        
            if (String.IsNullOrEmpty(data.GetCursorTime())) {
                reason.Append("\n Cursor time is null or blank");

            }

            if (reason.Length > 0)
            {
                return String.Format("Problems with your cursor input: {0}", reason.ToString());
            }

            return null;

        }
    }

}
