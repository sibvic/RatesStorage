using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProfitRobots.RatesStorage
{
    public class RatesStorage
    {
        private readonly string _path;
        RatesStorage(string path)
        {
            _path = path;
        }

        public static RatesStorage Create(string outputPath)
        {
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            return new RatesStorage(outputPath);
        }

        public bool DataExists(string provider, string symbol, DateTime date)
        {
            string fullpath = GetPathToData(provider, symbol, date);
            return File.Exists(fullpath);
        }

        public void SaveData(string provider, string symbol, List<Candle> prices)
        {
            var filePath = GetPathToData(provider, symbol, prices.First().Date);
            var symbolPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(symbolPath))
                Directory.CreateDirectory(symbolPath);
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
                List<Candle> candles = ReadData(fileName);
                if (!candles.Any())
                {
                    from = new DateTime(from.Year, from.Month, from.Day).ScrollTo(DayOfWeek.Monday);
                    var newFile = GetPathToData(provider, symbol, from);
                    while (from < to && newFile == fileName)
                    {
                        from = from.AddDays(7);
                    }
                    continue;
                }
                allCandles.AddRange(candles.Where(c => c.Date >= from && c.Date <= to));
                from = candles.Last().Date;
            }
            return allCandles;
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
            var files = Directory.GetFiles(path, "*.*").OrderBy(f => f);
            if (files.Count() == 0)
                return (null, null);
            var firstDate = ReadData(files.First()).Min(c => c.Date);
            var lastDate = ReadData(files.Last()).Max(c => c.Date);
            return (firstDate, lastDate);
        }

        private List<Candle> ReadData(string fileName)
        {
            if (!File.Exists(fileName))
                return new List<Candle>();
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
