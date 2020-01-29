using FFXIV_Data_Exporter.Library.Events;

using SaintCoinach;
using SaintCoinach.Xiv;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library
{
    public class Weather : IWeather
    {
        private readonly ARealmReversed _realm;
        private readonly ISendMessageEvent _sendMessageEvent;
        private readonly List<TerritoryType> _territories = new List<TerritoryType>();

        public Weather(IRealm realm, ISendMessageEvent sendMessageEvent)
        {
            _realm = realm.RealmReversed;
            _sendMessageEvent = sendMessageEvent;

            LoadZones();
        }

        public async Task GetWeatherAsync(DateTime dateTime, IEnumerable<string> zones, int forcastIntervals)
        {
            if (zones == null)
            {
                foreach (var territory in _territories)
                {
                    var eorzeaDateTime = new EorzeaDateTime(dateTime);
                    var zone = territory.PlaceName;
                    for (var i = 0; i < forcastIntervals; i++)
                    {
                        var weather = territory.WeatherRate.Forecast(eorzeaDateTime).Name;
                        var localTime = eorzeaDateTime.GetRealTime().ToLocalTime();
                        await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs($"{localTime}: {zone} - {weather}"));
                        eorzeaDateTime = Increment(eorzeaDateTime);
                    }
                }
            }
            else
            {
                foreach (var zone in zones)
                {
                    var eorzeaDateTime = new EorzeaDateTime(dateTime);
                    for (var i = 0; i < forcastIntervals; i++)
                    {
                        var weather = _territories.FirstOrDefault(_ => _.PlaceName.ToString() == zone).WeatherRate.Forecast(eorzeaDateTime).Name;
                        var localTime = eorzeaDateTime.GetRealTime().ToLocalTime();
                        await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs($"{localTime}: {zone} - {weather}"));
                        eorzeaDateTime = Increment(eorzeaDateTime);
                    }
                }
            }
        }

        public async Task GetMoonPhaseAsync(EorzeaDateTime eDate)
        {
            string[] moons = { "New Moon", "Waxing Crescent", "First Quarter", "Waxing Gibbous", "Full Moon", "Waning Gibbous", "Last Quarter", "Waning Crescent" };

            if (eDate == null)
            {
                eDate = new EorzeaDateTime(DateTime.Now);
            }

            var daysIntoCycle = DaysIntoLunarCycle(eDate);
            // 16 days until new or full moon.
            var percent = Math.Round(((daysIntoCycle % 16) / 16) * 100);
            // 4 days per moon.
            var index = Convert.ToInt32(Math.Floor(daysIntoCycle / 4));
            await _sendMessageEvent.OnSendMessageEventAsync(new SendMessageEventArgs($"Moon Phase: {moons[index]} {percent}%"));
        }

        private void LoadZones()
        {
            var territoryType = _realm.GameData.GetSheet("TerritoryType").ToList();
            var keyList = new List<int>()
            {128,129,130,131,132,133,134,135,137,138,139,140,141,145,146,147,148,152,153,154,155,156,180,250,339,340,341,397,398,399,400,401,402,418,419,478,612,613,614,620,621,622,628,635,641,759,813,814,815,816,817,818,819,820};
            foreach (var key in keyList)
            {
                _territories.Add((TerritoryType)territoryType.FirstOrDefault(_ => _.Key == key));
            }
        }

        private EorzeaDateTime Increment(EorzeaDateTime eorzeaDateTime)
        {
            eorzeaDateTime.Minute = 0;
            if (eorzeaDateTime.Bell < 8)
            {
                eorzeaDateTime.Bell = 8;
            }
            else if (eorzeaDateTime.Bell < 16)
            {
                eorzeaDateTime.Bell = 16;
            }
            else
            {
                eorzeaDateTime.Bell = 0;
                eorzeaDateTime.Sun++;
            }
            return eorzeaDateTime;
        }

        private double DaysIntoLunarCycle(EorzeaDateTime eDate)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var epochTimeFactor = (60.0 * 24.0) / 70.0; //20.571428571428573

            // Take an Eorzean DateTime and turn it into UTC
            var eorzeaToUTC = eDate.GetRealTime().ToUniversalTime();

            // Find total eorzian milliseconds since epoch
            var eorzeaTotalMilliseconds = eorzeaToUTC.Subtract(epoch).TotalMilliseconds * epochTimeFactor;

            // Get number of days into the cycle.
            // Moon is visible starting around 6pm. Change phase around noon when it can't be seen.
            // ((Total Eorzian Milliseconds since epoch / ([milliseconds in second] * [seconds in minute] * [minutes in hour] * [hours in day])) + mid-day) % [days in cycle(month)]
            return ((eorzeaTotalMilliseconds / (1000 * 60 * 60 * 24)) + .5) % 32;
        }
    }
}