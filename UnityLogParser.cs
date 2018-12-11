using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityTestRunnerResultsReporter.Entities;

namespace UnityTestRunnerResultsReporter
{
    /// <summary>
    /// The parser that reads Unity log for Unity information and test arguments
    /// </summary>
    class UnityLogParser
    {
        private const string unityInfoLinePrefix = "Built from";
        private const string testArgumentsHeader = "COMMAND LINE ARGUMENTS:";
        private const string testArgumentsTailPrefix = "Successfully changed project path";

        /// <summary>
        /// Parse production version information from unity log
        /// </summary>
        /// <param name="processor">The processer</param>
        public void TryParseProductVersion(UnityTestRunnerResultsProcessor processor)
        {
            var version = new ProductVersion();

            try
            {
                using (StreamReader sr = new StreamReader(processor.LogFilePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.StartsWith(unityInfoLinePrefix, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var parts = line.Split('\'');
                            version.branch = parts[1];
                            var unityInfo = parts[3].Split(' ');
                            version.unity_version = unityInfo[0];
                            version.revision = unityInfo[1];
                            version.revisionNumber = Convert.ToInt64(unityInfo[3]);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                processor.AddProducVersionToTestResult(version);
            }
        }

        /// <summary>
        /// Parse test arguments from unity log
        /// </summary>
        /// <param name="processor"></param>
        public void TryParseTestArguments(UnityTestRunnerResultsProcessor processor)
        {
            var arguments = new TestArguments();
            var listOfArguments = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(processor.LogFilePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.Contains(testArgumentsHeader))
                        {
                            string arg = sr.ReadLine();
                            arg = sr.ReadLine();    // The first line below header is Unity.exe, skip this
                            while (!arg.StartsWith(testArgumentsTailPrefix, StringComparison.CurrentCultureIgnoreCase))
                            {
                                listOfArguments.Add(arg);
                                arg = sr.ReadLine();
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                arguments.args = listOfArguments;
                processor.AddTestArgumentsToTestResult(arguments);
            }
        }
    }
}
