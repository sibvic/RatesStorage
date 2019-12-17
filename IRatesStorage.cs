using System;
using System.Collections.Generic;

namespace ProfitRobots.RatesStorage
{
    public interface IRatesStorage
    {
        bool DataExists(string provider, string symbol, DateTime date);
        /// <summary>
        /// Get dates available
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="symbol"></param>
        /// <returns>Start and end dates. Both dates will be null or not null.</returns>
        (DateTime?, DateTime?) GetDates(string provider, string symbol);
        IEnumerable<string> GetProviders();
        SymbolInfo GetSymbolInfo(string provider, string symbol);
        IEnumerable<string> GetSymbols(string provider);
        List<Candle> LoadData(string provider, string symbol, DateTime from, DateTime to);
        void SaveData(string provider, string symbol, List<Candle> prices);
        void SaveSymbolInfo(string provider, string symbol, SymbolInfo info);
    }
}