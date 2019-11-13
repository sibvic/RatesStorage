using System;
using System.Collections.Generic;

namespace ProfitRobots.RatesStorage
{
    public interface IRatesStorage
    {
        bool DataExists(string provider, string symbol, DateTime date);
        (DateTime?, DateTime?) GetDates(string provider, string symbol);
        SymbolInfo GetSymbolInfo(string provider, string symbol);
        List<Candle> LoadData(string provider, string symbol, DateTime from, DateTime to);
        void SaveData(string provider, string symbol, List<Candle> prices);
        void SaveSymbolInfo(string provider, string symbol, SymbolInfo info);
    }
}