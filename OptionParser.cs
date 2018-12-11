using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Options;

namespace UnityTestRunnerResultsReporter
{
    class OptionsParser
    {
        private bool help;
        private readonly string about = "The Unity Test Runner Results Reporter parses test results from Unity Test Runner (e.g. Unity.exe -runTests ...) and generates an html report.";

        private readonly string learnMore =
            "To learn more about the Unity Performance Benchmark Reporter visit the Unity Performance Benchmark Reporter GitHub wiki at [LinkToAdd].";

        private readonly string commandLineOptionFormat =
            "// Command line option format\r\n" +
            "--resultsPath=<Path to a directory where test result XML file and Unity log file can be find>\r\n " +
            "[--resultXMLName=\"Name of the test result XML file, reporter will look for TestResults.xml if no value\"]\r\n" +
            "[--unityLogName=\"Name of the unity log file, reporter will look for UnityLog.txt if no value\"]\r\n" +
            "[--reportdirpath=\"Path to where the report will be written\"]";

        private readonly string example1 =
            "// Run Unity Test Runner tests with default result file names\r\n" +
            "Unity.exe -projectPath G:\\MyUnityTest -logFile G:\\MyUnityTest\\Results\\UnityLog.txt -testPlatform Android -testResults G:\\MyUnityTest\\Results\\TestResults.xml -buildTarget Android -runTests\r\n" +
            "// Run reporter for test results\r\n" +
            "UnityTestRunnerResultsReporter.dll --resultsPath=G:\\MyUnityTest\\Results";

        private readonly string example2 = 
            "// Run Unity Test Runner tests with customized result file names\r\n" +
            "Unity.exe -projectPath G:\\MyUnityTest -logFile G:\\MyUnityTest\\Results\\myLog.txt -testPlatform Android -testResults G:\\MyUnityTest\\Results\\results.xml -buildTarget Android -runTests\r\n" +
            "// Run reporter for test results\r\n" +
            "UnityTestRunnerResultsReporter.dll --resultsPath=G:\\MyUnityTest\\Results --resultXMLName=results.xml --unityLogName=myLog.txt";

        public void ParseOptions(UnityTestRunnerResultsProcessor processor, IEnumerable<string> args)
        {
            var os = GetOptions(processor);

            try
            {
                var remaining = os.Parse(args);

                if (help)
                {
                    ShowHelp(string.Empty, os);
                }

                if (string.IsNullOrEmpty(processor.TestResultsPath) || string.IsNullOrEmpty(processor.ReportPath))
                {
                    ShowHelp("Missing required option --resultsPath=(directoryPath)", os);
                }

                if (remaining.Any())
                {
                    var errorMessage = string.Format("Unknown option: '{0}.\r\n'", remaining[0]);
                    ShowHelp(errorMessage, os);
                }
            }
            catch (Exception e)
            {
                ShowHelp(string.Format("Error encountered while parsing option: {0}.\r\n", e.Message), os);
            }
        }

        private OptionSet GetOptions(UnityTestRunnerResultsProcessor processor)
        {
            return new OptionSet()
                .Add("?|help|h", "Prints out the options.", option => help = option != null)
                .Add("resultsPath=", "REQUIRED - Path to a directory where test result XML file and Unity log file can be find. ",
                    resultsPath =>
                    {
                        processor.AddTestResultsPath(resultsPath, "resultsPath");
                    })
                .Add("resultXMLName|XMLName:", "OPTIONAL - Name of test result XML file name.",
                    xmlName =>
                    {
                        processor.AddFileName(xmlName, "resultXMLName");
                    })
                .Add("unityLogName|LogName:", "OPTIONAL - Name of Unity log file name.",
                    logName =>
                    {
                        processor.AddFileName(logName, "unityLogName");
                    })
                .Add("report|reportdirpath:", "OPTIONAL - Path to where the report will be written. Default is the given test results path.",
                    processor.AddReportDirPath);
        }

        private void ShowHelp(string message, OptionSet optionSet)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(message);
                Console.ResetColor();
            }

            Console.WriteLine(about + "\r\n");
            Console.WriteLine(learnMore + "\r\n");
            Console.WriteLine("Usage is:" + "\r\n");
            Console.WriteLine(commandLineOptionFormat + "\r\n");
            Console.WriteLine(example1 + "\r\n");
            Console.WriteLine(example2 + "\r\n");
            Console.WriteLine("Options: \r\n");
            optionSet.WriteOptionDescriptions(Console.Error);
            Environment.Exit(-1);
        }
    }
}
