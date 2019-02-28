using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityTestRunnerResultsReporter.Entities;

namespace UnityTestRunnerResultsReporter
{
    class UnityTestRunnerResultsProcessor
    {
        private const string testDataJsonFileName = "TestReportData.json";

        private string testResultPath;
        private string reportPath;
        private string xmlFilePath;
        private string logFilePath;
        private UnityTestRunnerTestResult testResults;
        private string testReportJson;

        public string TestResultsPath
        { get { return this.testResultPath; } }

        public string ReportPath
        { get { if (string.IsNullOrEmpty(this.reportPath)) { return this.testResultPath; } else { return this.reportPath; } } }

        public string XMLFilePath
        { get { return this.xmlFilePath; } }

        public string LogFilePath
        { get { return this.logFilePath; } }

        public string TestProjectName { get; set; }

        public UnityTestRunnerResultsProcessor()
        {
            this.testResults = new UnityTestRunnerTestResult();
            this.testResults.suites = new List<TestSuite>();
            TestSuite suite = new TestSuite();
            suite.artifacts = new List<string>();
            suite.minimalCommandLine = new List<string>();
            this.testResults.suites.Add(suite);
        }


        public void GenerateTestResultJson()
        {
            this.testReportJson = JsonConvert.SerializeObject(this.testResults);
            try
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(ReportPath, testDataJsonFileName), false))
                {
                    sw.Write(this.testReportJson);
                    sw.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void GenerateConsoleOutput()
        {
            var overall = this.testResults.summary.success ? "PASS" : "FAILED";
            Console.WriteLine(string.Format(@"
    ****************** Test result overview *********************
    Overall result: {0}
    Total Tests run: {1}, Passed: {2}, Failures:{3}, Errors: {4}, Inconclusives: {5}
    Total not run: {6}, Invalid: {7}, Ignored: {8}, Skipped: {9}
    *************************************************************
",
                overall,
                this.testResults.summary.testsCount,
                this.testResults.summary.successCount,
                this.testResults.summary.failedCount,
                this.testResults.summary.errorCount,
                this.testResults.summary.inconclusiveCount,
                this.testResults.summary.notRunCount,
                this.testResults.summary.inconclusiveCount,
                this.testResults.summary.ignoredCount,
                this.testResults.summary.skippedCount));
        }

        public void GenerateHtmlReport()
        {
            var templatePath = AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(Path.Combine(templatePath, "TestReport.html")) || !Directory.Exists(Path.Combine(templatePath, "TestReport.files")))
            {
                throw new Exception("Cannot find test report template files at " + templatePath);
            }

            try
            {
                using (StreamReader sr = new StreamReader(Path.Combine(templatePath, "TestReport.html")))
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(ReportPath, "TestReport.html")))
                    {
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if (line.Contains("TEST_DATA_HERE"))
                            {
                                sw.WriteLine(this.testReportJson);
                            }
                            else
                            {
                                sw.WriteLine(line);
                            }
                        }
                        sw.Flush();
                    }
                }
                this.DirectoryCopy(Path.Combine(templatePath, "TestReport.files"), Path.Combine(ReportPath, "TestReport.files"), true);
                Console.WriteLine("Test report generated: " + Path.Combine(ReportPath, "TestReport.html"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public void AddProducVersionToTestResult(ProductVersion version)
        {
            this.testResults.productVersion = version;
        }

        public void AddTestArgumentsToTestResult(TestArguments arguments)
        {
            this.testResults.arguments = arguments;
            bool hasProjectPath = false;
            foreach (string arg in arguments.args)
            {
                if (hasProjectPath)
                {
                    TestProjectName = arg;
                    break;
                }
                if (arg.Equals("-projectPath"))
                {
                    hasProjectPath = true;
                }
            }
            this.testResults.suites[0].name = TestProjectName;
            this.testResults.suites[0].minimalCommandLine = arguments.args;
        }

        public void AddTestSummaryToTestResult(TestSummary summary)
        {
            this.testResults.summary = summary;
        }

        public void AddTestCaseResultsToTestResult(IEnumerable<TestCase> testCaseResults)
        {
            this.testResults.suites[0].tests = new List<TestCase>(testCaseResults);
        }

        public void AddTestRunSummaryToTestResult(TestRunSummary testRunSummary)
        {
            this.testResults.suites[0].summary = testRunSummary;
        }

        public void AddTestResultsPath(string path, string optionName)
        {
            if (string.IsNullOrEmpty(path))
            {
                var e = new ArgumentNullException("Test result path is empty.");
            }

            if (string.IsNullOrEmpty(optionName))
            {
                throw new ArgumentNullException("There is no option given for result path.");
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(string.Format("Cannot find directory {0}", path));
            }

            this.testResultPath = path;
            this.xmlFilePath = Path.Combine(this.testResultPath, "TestResults.xml");
            this.logFilePath = Path.Combine(this.testResultPath, "UnityLog.txt");
            this.testResults.suites[0].artifacts.Add(this.xmlFilePath);
            this.testResults.suites[0].artifacts.Add(this.logFilePath);
        }

        public void AddFileName(string name, string optionName)
        {
            if (!File.Exists(Path.Combine(this.testResultPath, name)))
            {
                throw new FileNotFoundException(string.Format("Cannot find file {0}", Path.Combine(this.testResultPath, name)));
            }

            if (string.Equals(optionName, "resultXMLName"))
            {
                this.xmlFilePath = Path.Combine(this.testResultPath, name);
                this.testResults.suites[0].artifacts.Add(this.xmlFilePath);
            }
            else if (string.Equals(optionName, "unityLogName"))
            {
                this.logFilePath = Path.Combine(this.testResultPath, name);
                this.testResults.suites[0].artifacts.Add(this.logFilePath);
            }
        }

        public void AddReportDirPath(string reportDirectoryPath)
        {
            this.reportPath = reportDirectoryPath;
        }

        public bool GetOverallTestResults()
        {
            return this.testResults.summary.success;
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
