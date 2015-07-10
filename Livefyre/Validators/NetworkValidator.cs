using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Livefyre.Model;


namespace Livefyre.Validators
{

    public class NetworkValidator : Validator<NetworkData> {

        public string Validate(NetworkData data) {

            StringBuilder reason = new StringBuilder();

            if (String.IsNullOrEmpty(data.GetName()))
            {
                reason.Append("\n Name is null or blank.");

            } else if (!data.GetName().EndsWith(".fyre.co")) {
                reason.Append("\n Network name must end with '.fyre.co'.");

            }

            if (String.IsNullOrEmpty(data.GetKey()))
            {
                reason.Append("\n Key is null or blank.");

            }


            if (reason.Length > 0)
            {
                return String.Format("Problems with your network input: {0}", reason.ToString());
            }

            return null;
        }

    }

}
