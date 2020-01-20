using SaintCoinach;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library
{
    public interface IWeather
    {
        List<string> GetWeather(DateTime dateTime, IEnumerable<string> zones, int forcastIntervals);
        Task<List<string>> GetWeatherAsync(DateTime dateTime, IEnumerable<string> zones, int forcastIntervals);

        // Overloads
        List<string> GetWeather() => GetWeather(DateTime.Now, null, 1);
        List<string> GetWeather(DateTime dateTime) => GetWeather(dateTime, null, 1);
        List<string> GetWeather(IEnumerable<string> zones) => GetWeather(DateTime.Now, zones, 1);
        List<string> GetWeather(int forcastIntervals) => GetWeather(DateTime.Now, null, forcastIntervals);
        List<string> GetWeather(DateTime dateTime, IEnumerable<string> zones) => GetWeather(dateTime, zones, 1);
        List<string> GetWeather(DateTime dateTime, int forcastIntervals) => GetWeather(dateTime, null, forcastIntervals);
        List<string> GetWeather(IEnumerable<string> zones, int forcastIntervals) => GetWeather(DateTime.Now, zones, forcastIntervals);
        List<string> GetWeather(EorzeaDateTime eorzeaDateTime) => GetWeather(eorzeaDateTime.GetRealTime(), null, 1);
        List<string> GetWeather(EorzeaDateTime eorzeaDateTime, IEnumerable<string> zones) => GetWeather(eorzeaDateTime.GetRealTime(), zones, 1);
        List<string> GetWeather(EorzeaDateTime eorzeaDateTime, int forcastIntervals) => GetWeather(eorzeaDateTime.GetRealTime(), null, forcastIntervals);
        List<string> GetWeather(EorzeaDateTime eorzeaDateTime, IEnumerable<string> zones, int forcastIntervals) => GetWeather(eorzeaDateTime.GetRealTime(), zones, forcastIntervals);

        // Async Overloads
        async Task<List<string>> GetWeatherAsync() => await GetWeatherAsync(DateTime.Now, null, 1);
        async Task<List<string>> GetWeatherAsync(DateTime dateTime) => await GetWeatherAsync(dateTime, null, 1);
        async Task<List<string>> GetWeatherAsync(IEnumerable<string> zones) => await GetWeatherAsync(DateTime.Now, zones, 1);
        async Task<List<string>> GetWeatherAsync(int forcastIntervals) => await GetWeatherAsync(DateTime.Now, null, forcastIntervals);
        async Task<List<string>> GetWeatherAsync(DateTime dateTime, IEnumerable<string> zones) => await GetWeatherAsync(dateTime, zones, 1);
        async Task<List<string>> GetWeatherAsync(DateTime dateTime, int forcastIntervals) => await GetWeatherAsync(dateTime, null, forcastIntervals);
        async Task<List<string>> GetWeatherAsync(IEnumerable<string> zones, int forcastIntervals) => await GetWeatherAsync(DateTime.Now, zones, forcastIntervals);
        async Task<List<string>> GetWeatherAsync(EorzeaDateTime eorzeaDateTime) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), null, 1);
        async Task<List<string>> GetWeatherAsync(EorzeaDateTime eorzeaDateTime, IEnumerable<string> zones) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), zones, 1);
        async Task<List<string>> GetWeatherAsync(EorzeaDateTime eorzeaDateTime, int forcastIntervals) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), null, forcastIntervals);
        async Task<List<string>> GetWeatherAsync(EorzeaDateTime eorzeaDateTime, IEnumerable<string> zones, int forcastIntervals) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), zones, forcastIntervals);
    }
}