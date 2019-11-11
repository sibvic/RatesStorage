namespace ProfitRobots.RatesStorage
{
    public class SymbolInfo
    {
        public string ContractCurrency { get; set; }
        public string ProfitCurrency { get; set; }
        public int BaseUnitSize { get; set; }
        public double ContractMultiplier { get; set; }
        public int InstrumentType { get; set; }
        public double MMR { get; set; }
        public double PipSize { get; set; }
        public int Precision { get; set; }
        public string Name { get; set; }
    }
}
