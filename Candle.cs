using System;

namespace ProfitRobots.RatesStorage
{
    public class Candle
    {
        public DateTime Date { get; set; }
        public OHLC Bid { get; set; }
        public OHLC Ask { get; set; }
        public long Volume { get; set; }

        public override bool Equals(object obj)
        {
            // Assume it's the same timeframe and symbol
            // and the prices are historical. Compare date only.
            if (obj is Candle other)
                return Date == other.Date;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Date.GetHashCode();
        }
    }
}
