using ProfitRobots.RatesStorage.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace ProfitRobots.RatesStorage
{
    class RatesStorage : IRatesStorage
    {
        private readonly string _path;
        public RatesStorage(string path)
        {
            _path = path;
        }

        public bool DataExists(string provider, string symbol, DateTime date)
        {
            string fullpath = GetPathToData(provider, symbol, date);
            return File.Exists(fullpath);
        }

        public void SaveData(string provider, string symbol, List<Candle> prices)
        {
            var filePath = GetPathToData(provider, symbol, prices.First().Date);
            EnsurePathExists(filePath);
            using (var stream = new StreamWriter(filePath))
            {
                foreach (var price in prices)
                {
                    CandleSerializer.Serialize(price, ';', stream);
                }
                stream.Flush();
                stream.Close();
            }
        }

        private static void EnsurePathExists(string filePath)
        {
            var symbolPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(symbolPath))
                Directory.CreateDirectory(symbolPath);
        }

        /// Loads data from the storage. 
        /// The missing data will be skipped.
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<Candle> LoadData(string provider, string symbol, DateTime from, DateTime to)
        {
            if (from > to)
                throw new ArgumentOutOfRangeException("From date should be earlier that to date.");

            List<Candle> allCandles = new List<Candle>();
            while (from < to)
            {
                var fileName = GetPathToData(provider, symbol, from);
                var candles = ReadData(fileName).Where(c => c.Date >= from && c.Date <= to);
                if (candles.Any())
                    allCandles.AddRange(candles);
                from = GetNextDate(from);
            }
            return allCandles;
        }

        private static DateTime GetNextDate(DateTime from)
        {
            var newData = new DateTime(from.Year, from.Month, from.Day);
            if (newData.DayOfWeek == DayOfWeek.Monday)
                return newData.AddDays(1).ScrollTo(DayOfWeek.Monday);
            return newData.ScrollTo(DayOfWeek.Monday);
        }

        /// <summary>
        /// Get available dates for the symbol.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public (DateTime?, DateTime?) GetDates(string provider, string symbol)
        {
            var path = GetPathToSymbol(provider, symbol);
            if (!Directory.Exists(path))
                return (null, null);
            var files = Directory.GetFiles(path, "*.csv");
            if (files.Count() == 0)
                return (null, null);

            try
            {
                (string first, string last) = GetFirstLastFiles(files.ToList());
                var firstDate = ReadData(first).Min(c => c.Date);
                var lastDate = ReadData(last).Max(c => c.Date);
                return (firstDate, lastDate);
            }
            catch (NoDataException)
            {
                return (null, null);
            }
        }

        Regex _filesPattern = new Regex("(\\d+)-(\\d+)");
        private (string first, string last) GetFirstLastFiles(List<string> files)
        {
            string first = null;
            int firstYear = 0;
            int firstWeek = 0;

            string last = null;
            int lastYear = 0;
            int lastWeek = 0;
            foreach (var file in files)
            {
                var match = _filesPattern.Match(file);
                if (!match.Success)
                    continue;
                if (!int.TryParse(match.Groups[1].Value, out int year) || !int.TryParse(match.Groups[2].Value, out int week))
                    continue;
                if (first == null || firstYear > year || (firstYear == year && firstWeek > week))
                {
                    first = file;
                    firstYear = year;
                    firstWeek = week;
                }
                if (last == null || lastYear < year || (lastYear == year && lastWeek < week))
                {
                    last = file;
                    lastYear = year;
                    lastWeek = week;
                }

            }
            return (first, last);
        }


        public void SaveSymbolInfo(string provider, string symbol, SymbolInfo info)
        {
            info.Name = symbol;
            var rootFolder = GetPathToSymbol(provider, symbol);
            var infoFileName = Path.Combine(rootFolder, "info.json");
            EnsurePathExists(infoFileName);
            if (File.Exists(infoFileName))
            {
                if (!info.WithoutHistory)
                {
                    return;
                }
                File.Delete(infoFileName);
            }

            var json = JsonConvert.SerializeObject(info);
            File.WriteAllText(infoFileName, json);
        }

        public SymbolInfo GetSymbolInfo(string provider, string symbol)
        {
            var rootFolder = GetPathToSymbol(provider, symbol);
            var infoFileName = Path.Combine(rootFolder, "info.json");
            if (!File.Exists(infoFileName))
                return null;

            var json = File.ReadAllText(infoFileName);
            var info = JsonConvert.DeserializeObject<SymbolInfo>(json);
            info.Provider = provider;
            return info;
        }

        /// <summary>
        /// Reads data from the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="NoDataException">When no data present</exception>
        /// <returns></returns>
        private List<Candle> ReadData(string fileName)
        {
            if (!File.Exists(fileName))
                throw new NoDataException();
            var lines = File.ReadAllLines(fileName);
            List<Candle> data = new List<Candle>();
            foreach (var line in lines)
            {
                var candle = CandleSerializer.Deserialize(line, ';');
                data.Add(candle);
            }
            return data;
        }
        private string GetPathToSymbol(string provider, string symbol)
        {
            return Path.Combine(_path, provider, symbol.Replace("/", ""));
        }

        private string GetPathToData(string provider, string symbol, DateTime date)
        {
            var filename = date.Year + "-" + date.WeekNumber() + ".csv";
            var fullpath = Path.Combine(GetPathToSymbol(provider, symbol), filename);
            return fullpath;
        }
    }
}
