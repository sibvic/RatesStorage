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
            stream.Write(candle.Ask.Open);
            stream.Write(separator);
            stream.Write(candle.Ask.High);
            stream.Write(separator);
            stream.Write(candle.Ask.Low);
            stream.Write(separator);
            stream.Write(candle.Ask.Close);
            stream.Write(separator);
            stream.WriteLine(candle.Volume.ToString());
        }

        public static Candle Deserialize(string text, char separator)
        {
            var items = text.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            return new Candle()
            {
                Date = DateTime.Parse(items[0]),
                Bid = new OHLC()
                {
                    Open = double.Parse(items[1]),
                    High = double.Parse(items[2]),
                    Low = double.Parse(items[3]),
                    Close = double.Parse(items[4])
                },
                Ask = new OHLC()
                {
                    Open = double.Parse(items[5]),
                    High = double.Parse(items[6]),
                    Low = double.Parse(items[7]),
                    Close = double.Parse(items[8]),
                },
                Volume = long.Parse(items[9])
            };
        }
    }
}
