using System.IO;

namespace ProfitRobots.RatesStorage
{
    public class RatesStorageFactory
    {
        public static IRatesStorage Create(string outputPath)
        {
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            return new RatesStorage(outputPath);
        }
    }
}
