using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class GlobalVar
    {
        /// <summary>
        /// Global variable storing important stuff.
        /// </summary>
        public static string screen;


        public static string ScreenSession
        {
            get
            {
                return screen;

            }
            set
            {
                screen = value;

            }

        }

    }
}
