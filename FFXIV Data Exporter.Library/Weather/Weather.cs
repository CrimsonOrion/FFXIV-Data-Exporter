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
        private readonly List<TerritoryType> _territories = new List<TerritoryType>();

        public Weather(Realm realm)
        {
            _realm = realm.RealmReversed;

            LoadZones();

        }

        public async Task<List<string>> GetWeatherAsync(DateTime dateTime, IEnumerable<string> zones, int forcastIntervals) => await Task.Run(() => GetWeather(dateTime, zones, forcastIntervals));

        public List<string> GetWeather(DateTime dateTime, IEnumerable<string> zones, int forcastIntervals)
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