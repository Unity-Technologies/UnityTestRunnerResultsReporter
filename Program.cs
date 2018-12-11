using System;

namespace UnityTestRunnerResultsReporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var processor = new UnityTestRunnerResultsProcessor();
            var optionsParser = new OptionsParser();

            Console.WriteLine("Reading parameters and validating test results....");
            optionsParser.ParseOptions(processor, args);

            var logParser = new UnityLogParser();
            var xmlParser = new TestResultXmlParser();

            Console.WriteLine("Trying to parse Unity version and test metadata...");
            logParser.TryParseProductVersion(processor);
            logParser.TryParseTestArguments(processor);
            Console.WriteLine("Trying to parse test results...");
            xmlParser.ParseTestResults(processor);

            Console.WriteLine("Generating test result JSON file...");
            processor.GenerateTestResultJson();
            processor.GenerateConsoleOutput();
            Console.WriteLine("Generating HTML test report...");
            processor.GenerateHtmlReport();

            if (processor.GetOverallTestResults())
            {
                Environment.Exit(0);
            }
            else
            {
                Environment.Exit(1);
            }
        }
    }
}
