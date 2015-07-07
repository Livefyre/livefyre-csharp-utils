using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Livefyre.Type
{

    // THIS MAY BE A MESS OR MAY WORK...

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
    

        public static string fromNum(int num) {
        //WATCH THIS TYPE!
        //public static SubscriptionType fromNum(int num) {
            foreach (var name in Enum.GetNames(typeof(Values)))
            {
                if ((int)Enum.Parse(typeof(Values), name) == num)
                {
                    return name;
                }
            }

            throw new ArgumentOutOfRangeException("num", String.Format("No constant with value {0} found!", num));
        }


    }

}
