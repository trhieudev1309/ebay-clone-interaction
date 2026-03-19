using System.Globalization;

namespace EbayChat.Utils
{
    public class DateHelper
    {
        public static string FormatChatTime(string dateTimeString)
        {
            if (!DateTime.TryParseExact(
                    dateTimeString,
                    "M/d/yyyy h:mm:ss tt",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsedDate))
            {
                return dateTimeString; // fallback if parsing fails
            }

            DateTime now = DateTime.UtcNow.AddHours(7);
            string dayPart;

            if (parsedDate.Date == now.Date)
                dayPart = "Today";
            else if (parsedDate.Date == now.Date.AddDays(-1))
                dayPart = "Yesterday";
            else
                dayPart = parsedDate.ToString("MMM d", CultureInfo.InvariantCulture);

            string timePart = parsedDate.ToString("h:mm tt", CultureInfo.InvariantCulture);

            return $"{timePart} | {dayPart}";
        }
    }
}
