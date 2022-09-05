using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{

    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns a string representation of this date/time. If the
        /// value is close to now, a relative description is returned.
        /// </summary>
        public static string ToRelativeString(this DateTime dt)
        {
            TimeSpan span = (DateTime.Now - dt);

            // Normalize time span
            bool future = false;
            if (span.TotalSeconds < 0)
            {
                // In the future
                span = -span;
                future = true;
            }

            // Test for Now
            double totalSeconds = span.TotalSeconds;
            if (totalSeconds < 0.9)
            {
                return "Now";
            }

            // Date/time near current date/time
            string format = (future) ? "In {0}{1}" : "{0}{1} ago";
            if (totalSeconds < 55)
            {
                // Seconds
                int seconds = Math.Max(1, span.Seconds);
                return String.Format(format, seconds,
                    //(seconds == 1) ? "second" : "seconds");
                    (seconds == 1) ? "s" : "s");
            }

            if (totalSeconds < (55 * 60))
            {
                // Minutes
                int minutes = Math.Max(1, span.Minutes);
                return String.Format(format, minutes,
                    //(minutes == 1) ? "minute" : "minutes");
                    (minutes == 1) ? "m" : "m");
            }
            if (totalSeconds < (24 * 60 * 60))
            {
                // Hours
                int hours = Math.Max(1, span.Hours);
                return String.Format(format, hours,
                    //(hours == 1) ? "hour" : "hours");
                (hours == 1) ? "h" : "h");
            }

            // Format both date and time
            if (totalSeconds < (48 * 60 * 60))
            {
                // 1 Day
                format = (future) ? "Tomorrow" : "Yesterday";
            }
            else if (totalSeconds < (3 * 24 * 60 * 60))
            {
                // 2 Days
                //format = String.Format(format, 2, "days");
                format = String.Format(format, 2, "d");
            }
            else
            {
                // Absolute date
                if (dt.Year == DateTime.Now.Year)
                    format = String.Format("{0:d MMM}", dt);// dt.ToString(@"d MMM");
                else
                    format = String.Format("{0:d MMM yy}", dt);// dt.ToString(@"d MMM'yy");
            }

            // Add time
            return String.Format("{0} {1:h:mmtt}", format, dt);
        }
    }
}
