using SaintCoinach;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library
{
    public interface IWeather
    {
        Task GetWeatherAsync(DateTime dateTime, IEnumerable<string> zones, int forcastIntervals);
        Task GetMoonPhaseAsync(EorzeaDateTime eDate);

        // Async Overloads
        async Task GetWeatherAsync() => await GetWeatherAsync(DateTime.Now, null, 1);
        async Task GetWeatherAsync(DateTime dateTime) => await GetWeatherAsync(dateTime, null, 1);
        async Task GetWeatherAsync(IEnumerable<string> zones) => await GetWeatherAsync(DateTime.Now, zones, 1);
        async Task GetWeatherAsync(int forcastIntervals) => await GetWeatherAsync(DateTime.Now, null, forcastIntervals);
        async Task GetWeatherAsync(DateTime dateTime, IEnumerable<string> zones) => await GetWeatherAsync(dateTime, zones, 1);
        async Task GetWeatherAsync(DateTime dateTime, int forcastIntervals) => await GetWeatherAsync(dateTime, null, forcastIntervals);
        async Task GetWeatherAsync(IEnumerable<string> zones, int forcastIntervals) => await GetWeatherAsync(DateTime.Now, zones, forcastIntervals);
        async Task GetWeatherAsync(EorzeaDateTime eorzeaDateTime) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), null, 1);
        async Task GetWeatherAsync(EorzeaDateTime eorzeaDateTime, IEnumerable<string> zones) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), zones, 1);
        async Task GetWeatherAsync(EorzeaDateTime eorzeaDateTime, int forcastIntervals) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), null, forcastIntervals);
        async Task GetWeatherAsync(EorzeaDateTime eorzeaDateTime, IEnumerable<string> zones, int forcastIntervals) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), zones, forcastIntervals);

        async Task GetMoonPhaseAsync() => await GetMoonPhaseAsync(null);
    }
}