using SaintCoinach;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIV_Data_Exporter.Library
{
    public interface IWeather
    {
        Task GetWeatherAsync(DateTime dateTime, IEnumerable<string> zones, int forcastIntervals, CancellationToken cancellationToken);
        void GetMoonPhase(EorzeaDateTime eDate);

        // Async Overloads
        async Task GetWeatherAsync(CancellationToken cancellationToken) => await GetWeatherAsync(DateTime.Now, null, 1, cancellationToken);
        async Task GetWeatherAsync(DateTime dateTime, CancellationToken cancellationToken) => await GetWeatherAsync(dateTime, null, 1, cancellationToken);
        async Task GetWeatherAsync(IEnumerable<string> zones, CancellationToken cancellationToken) => await GetWeatherAsync(DateTime.Now, zones, 1, cancellationToken);
        async Task GetWeatherAsync(int forcastIntervals, CancellationToken cancellationToken) => await GetWeatherAsync(DateTime.Now, null, forcastIntervals, cancellationToken);
        async Task GetWeatherAsync(DateTime dateTime, IEnumerable<string> zones, CancellationToken cancellationToken) => await GetWeatherAsync(dateTime, zones, 1, cancellationToken);
        async Task GetWeatherAsync(DateTime dateTime, int forcastIntervals, CancellationToken cancellationToken) => await GetWeatherAsync(dateTime, null, forcastIntervals, cancellationToken);
        async Task GetWeatherAsync(IEnumerable<string> zones, int forcastIntervals, CancellationToken cancellationToken) => await GetWeatherAsync(DateTime.Now, zones, forcastIntervals, cancellationToken);
        async Task GetWeatherAsync(EorzeaDateTime eorzeaDateTime, CancellationToken cancellationToken) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), null, 1, cancellationToken);
        async Task GetWeatherAsync(EorzeaDateTime eorzeaDateTime, IEnumerable<string> zones, CancellationToken cancellationToken) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), zones, 1, cancellationToken);
        async Task GetWeatherAsync(EorzeaDateTime eorzeaDateTime, int forcastIntervals, CancellationToken cancellationToken) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), null, forcastIntervals, cancellationToken);
        async Task GetWeatherAsync(EorzeaDateTime eorzeaDateTime, IEnumerable<string> zones, int forcastIntervals, CancellationToken cancellationToken) => await GetWeatherAsync(eorzeaDateTime.GetRealTime(), zones, forcastIntervals, cancellationToken);

        void GetMoonPhase() => GetMoonPhase(null);
    }
}