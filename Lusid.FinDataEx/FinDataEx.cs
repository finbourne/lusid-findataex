using System;

namespace Lusid.FinDataEx
{
    class Program
    {
        static void Main(string[] args)
        {
            string bbgDataType = args[0];
            string[] figiIds = GetFigiIds(args[1]);
            string outputPath = args[2];
            
            
        }

        static string[] GetFigiIds(string figiIdArg)
        {
            return figiIdArg.Split("|");
        }
    }
}