using System;
using System.IO;

namespace ProfitRobots.RatesStorage
{
    class CandleSerializer
    {
        public static void Serialize(Candle candle, char separator, StreamWriter stream)
        {
            stream.Write(candle.Date);
            stream.Write(separator);
            stream.Write(candle.Bid.Open);
            stream.Write(separator);
            stream.Write(candle.Bid.High);
            stream.Write(separator);
            stream.Write(candle.Bid.Low);
            stream.Write(separator);
            stream.Write(candle.Bid.Close);
            stream.Write(separator);
            if (candle.Ask != null)
            {
                stream.Write(candle.Ask.Open);
                stream.Write(separator);
                stream.Write(candle.Ask.High);
                stream.Write(separator);
                stream.Write(candle.Ask.Low);
                stream.Write(separator);
                stream.Write(candle.Ask.Close);
                stream.Write(separator);
            }
            stream.WriteLine(candle.Volume.ToString());
        }

        public static Candle Deserialize(string text, char separator)
        {
            var items = text.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            OHLC ask = null;
            double volume;
            if (items.Length == 10)
            {
                volume = double.Parse(items[9]);
                ask = ParseOHLC(items[5], items[6], items[7], items[8]);
            }
            else
            {
                volume = double.Parse(items[5]);
            }
            return new Candle()
            {
                Date = DateTime.Parse(items[0]),
                Bid = ParseOHLC(items[1], items[2], items[3], items[4]),
                Ask = ask,
                Volume = volume
            };
        }
        
        private static OHLC ParseOHLC(string open, string high, string low, string close)
        {
            return new OHLC()
            {
                Open = decimal.Parse(open),
                High = decimal.Parse(high),
                Low = decimal.Parse(low),
                Close = decimal.Parse(close),
            };
        }
    }
}
