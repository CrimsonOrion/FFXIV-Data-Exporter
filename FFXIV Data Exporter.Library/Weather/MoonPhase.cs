using SaintCoinach;

using System;

namespace FFXIV_Data_Exporter.Library
{
    public static class MoonPhase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static string[] moons = { "New Moon", "Waxing Crescent", "First Quarter", "Waxing Gibbous", "Full Moon", "Waning Gibbous", "Last Quarter", "Waning Crescent" };

        public static string CurrentMoonPhase(EorzeaDateTime eDate = null)
        {
            if (eDate == null)
            {
                eDate = new EorzeaDateTime(DateTime.Now);
            }

            var daysIntoCycle = DaysIntoLunarCycle(eDate);
            // 16 days until new or full moon.
            var percent = Math.Round(((daysIntoCycle % 16) / 16) * 100);
            // 4 days per moon.
            var index = Convert.ToInt32(Math.Floor(daysIntoCycle / 4));
            return $"Moon Phase: {moons[index]} {percent}%";
        }

        private static double DaysIntoLunarCycle(EorzeaDateTime eDate)
        {
            var epochTimeFactor = (60.0 * 24.0) / 70.0; //20.571428571428573

            // Take an Eorzean DateTime and turn it into UTC
            var eorzeaToUTC = eDate.GetRealTime().ToUniversalTime();

            // Find total eorzian milliseconds since epoch
            var eorzeaTotalMilliseconds = eorzeaToUTC.Subtract(_epoch).TotalMilliseconds * epochTimeFactor;

            // Get number of days into the cycle.
            // Moon is visible starting around 6pm. Change phase around noon when it can't be seen.
            // ((Total Eorzian Milliseconds since epoch / ([milliseconds in second] * [seconds in minute] * [minutes in hour] * [hours in day])) + mid-day) % [days in cycle(month)]
            return ((eorzeaTotalMilliseconds / (1000 * 60 * 60 * 24)) + .5) % 32;
        }
    }
}