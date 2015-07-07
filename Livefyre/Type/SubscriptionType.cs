using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Type
{
   public class SubscriptionType
    {
        //public static readonly int PersonalStream = 1;

        public enum Values
        {
            PersonalStream = 1
        }

       /*
        public static IEnumerable<int> Values
        {
            get
            {
                yield return PersonalStream;
            }
        }
       */


        private int val;
    
        private SubscriptionType(int val) {
            this.val = val;
        }
    
        // name comes from Java enum internal prop name
        public string toString() {
            return this.GetType().ToString();
        }
    
        public static SubscriptionType fromNum(int num) {




            // does the .values() call reveal any Constants from the Java side?
            /*
            for (SubscriptionType e : SubscriptionType.values()) {
                if (num == e.val) {
                    return e;
                }
            }
             */
            foreach (int val in Enum.GetValues(Values))
            {
            }


            throw new ArgumentOutOfRangeException("num", String.Format("No constant with value {0} found!", num));
        }
    }

}
