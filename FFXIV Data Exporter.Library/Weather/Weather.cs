using SaintCoinach;
using SaintCoinach.Xiv;

using System;
using System.Collections.Generic;
using System.Linq;

namespace FFXIV_Data_Exporter.Library
{
    public class Weather
    {
        private readonly ARealmReversed _realm;
        private readonly List<TerritoryType> _territories = new List<TerritoryType>();

        public Weather(Realm realm)
        {
            _realm = realm.RealmReversed;

            LoadZones();

            var zones = new List<string> { "Yanxia", "Lower La Noscea" };
            var weather = GetWeather(DateTime.Now, zones, 10);
        }

        public List<string> GetWeather() => GetWeather(DateTime.Now, null, 1);
        //public List<string> GetWeather(DateTime dateTime) => GetWeather(dateTime, null, 1);
        //public List<string> GetWeather(IEnumerable<string> zones) => GetWeather(DateTime.Now, zones, 1);
        //public List<string> GetWeather(int forcastIntervals) => GetWeather(DateTime.Now, null, forcastIntervals);
        //public List<string> GetWeather(DateTime dateTime, IEnumerable<string> zones) => GetWeather(dateTime, zones, 1);
        //public List<string> GetWeather(DateTime dateTime, int forcastIntervals) => GetWeather(dateTime, null, forcastIntervals);
        //public List<string> GetWeather(IEnumerable<string> zones, int forcastIntervals) => GetWeather(DateTime.Now, zones, forcastIntervals);
        public List<string> GetWeather(EorzeaDateTime eorzeaDateTime, IEnumerable<string> zones = null, int forcastIntervals = 1) => GetWeather(eorzeaDateTime.GetRealTime(), zones, forcastIntervals);

        public List<string> GetWeather(DateTime dateTime, IEnumerable<string> zones = null, int forcastIntervals = 1)
        {
            var weatherForcast = new List<string>();
            
            if (zones == null)
            {
                foreach (var territory in _territories)
                {
                    var eorzeaDateTime = new EorzeaDateTime(dateTime);
                    var zone = territory.PlaceName;
                    for (int i = 0; i < forcastIntervals; i++)
                    {
                        var weather = territory.WeatherRate.Forecast(eorzeaDateTime).Name;
                        var localTime = eorzeaDateTime.GetRealTime().ToLocalTime();
                        weatherForcast.Add($"{localTime}: {zone} - {weather}");
                        eorzeaDateTime = Increment(eorzeaDateTime);
                    }
                }
            }
            else
            {
                foreach (var zone in zones)
                {
                    var eorzeaDateTime = new EorzeaDateTime(dateTime);
                    for (int i = 0; i < forcastIntervals; i++)
                    {
                        var weather = _territories.FirstOrDefault(_ => _.PlaceName.ToString() == zone).WeatherRate.Forecast(eorzeaDateTime).Name;
                        var localTime = eorzeaDateTime.GetRealTime().ToLocalTime();
                        weatherForcast.Add($"{localTime}: {zone} - {weather}");
                        eorzeaDateTime = Increment(eorzeaDateTime);
                    }
                }
            }

            return weatherForcast;
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
    }
}