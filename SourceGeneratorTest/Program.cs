using SourceGeneratorPOC;
using StartupExtensions;
using System;

namespace SourceGeneratorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            StartupRunner.Run();
        }
    }

    public class Type1 : IStartup
    {
        public void Execute()
        {
            Console.WriteLine($"Executing startup for {nameof(Type1)}");
        }
    }

    public class Type3 : IStartup
    {
        public void Execute()
        {
            Console.WriteLine($"Executing startup for {nameof(Type3)}");
        }
    }
}
