using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityTestRunnerResultsReporter.Entities;

namespace UnityTestRunnerResultsReporter
{
    class TestResultXmlParser
    {
        private string testProject = string.Empty;

        public void ParseTestResults(UnityTestRunnerResultsProcessor processor)
        {
            this.testProject = processor.TestProjectName;
            var testResultXML = TryLoadResultXmlFile(processor.XMLFilePath);
            
            var testSummary = TryParseTestSummary(testResultXML);
            processor.AddTestSummaryToTestResult(testSummary);

            var testCaseResult = TryParseTestCaseResults(testResultXML);
            processor.AddTestCaseResultsToTestResult(testCaseResult);

            var testRunSummary = TryParseTestRunSummary(testResultXML, testSummary);
            processor.AddTestRunSummaryToTestResult(testRunSummary);
        }

        private TestRunSummary TryParseTestRunSummary(XDocument xDocument, TestSummary summary)
        {
            try
            {
                var testRunSummary = new TestRunSummary();
                testRunSummary.failedCount = summary.failedCount;
                testRunSummary.inconclusiveCount = summary.inconclusiveCount;
                testRunSummary.notRunCount = summary.notRunCount;
                testRunSummary.skippedCount = summary.skippedCount;
                testRunSummary.successCount = summary.successCount;
                testRunSummary.testsCount = summary.testsCount;
                var output = xDocument.Descendants("test-run").ToList<XElement>();
                var resultInfo = output[0];
                testRunSummary.time = Convert.ToInt64(Convert.ToDouble(resultInfo.Attribute("duration").Value) * 100000);
                if (summary.success)
                {
                    testRunSummary.result = 4;
                }
                else
                {
                    if (summary.failedCount != 0)
                        testRunSummary.result = 5;
                    else if (summary.inconclusiveCount != 0)
                        testRunSummary.result = 0;
                }
                return testRunSummary;
            }
            catch (Exception e)
            {
                string errMsg = "Failed to parse test run summary.";
                WriteExceptionConsoleErrorMessage(errMsg, e);
                throw;
            }
        }

        private IEnumerable<TestCase> TryParseTestCaseResults(XDocument xDocument)
        {
            try
            {
                List<TestCase> testCases = new List<TestCase>();
                var output = xDocument.Descendants("test-case");
                foreach (XElement testCase in output)
                {
                    var testCaseResult = GetTestCaseResult(testCase);
                    testCases.Add(testCaseResult);
                }
                return testCases;
            }
            catch (Exception e)
            {
                var errMsg = "Failed to parse test case results.";
                WriteExceptionConsoleErrorMessage(errMsg, e);
                throw;
            }
        }

        private TestCase GetTestCaseResult(XElement tc)
        {
            TestCase testCaseResult = new TestCase();
            testCaseResult.name = tc.Attribute("fullname").Value;
            testCaseResult.fixture = this.testProject;
            testCaseResult.state = ConvertToTestState(tc.Attribute("result").Value);
            testCaseResult.time = Convert.ToInt64(Convert.ToDouble(tc.Attribute("duration").Value) * 1000);
            testCaseResult.durationMicroseconds = Convert.ToInt64(Convert.ToDouble(tc.Attribute("duration").Value) * 1000000);
            testCaseResult.className = tc.Attribute("classname").Value;
            if (testCaseResult.state != 4)
            {
                var messageElement = tc.Descendants("message");
                if (messageElement != null && messageElement.Count() >= 1)
                {
                    var message = messageElement.First<XElement>().FirstNode;
                    testCaseResult.message = message == null ? string.Empty : message.ToString();
                }
                var traceElement = tc.Descendants("stack-trace");
                if (traceElement != null && traceElement.Count() >= 1)
                {
                    var trace = traceElement.First<XElement>().FirstNode;
                    testCaseResult.stackTrace = trace == null ? null : trace.ToString();
                }
            }
            return testCaseResult;
        }

        private int ConvertToTestState(string value)
        {
            switch (value)
            {
                case "Passed":
                    return 4;
                case "Failed":
                    return 5;
                case "Skipped":
                    return 2;
                case "Inconclusive":
                    return 0;
                default:
                    return 0;
            }
        }

        private TestSummary TryParseTestSummary(XDocument xDocument)
        {
            try
            {
                TestSummary summary = new TestSummary();
                var output = xDocument.Descendants("test-run").ToList<XElement>();
                var resultInfo = output[0];
                summary.testsCount = Convert.ToInt32(resultInfo.Attribute("total").Value);
                summary.successCount = Convert.ToInt32(resultInfo.Attribute("passed").Value);
                summary.failedCount = Convert.ToInt32(resultInfo.Attribute("failed").Value);
                summary.inconclusiveCount = Convert.ToInt32(resultInfo.Attribute("inconclusive").Value);
                summary.skippedCount = Convert.ToInt32(resultInfo.Attribute("skipped").Value);
                summary.notRunCount = summary.skippedCount + summary.ignoredCount;
                summary.success = summary.failedCount == 0 && summary.inconclusiveCount == 0 && summary.testsCount != 0;

                return summary;
            }
            catch (Exception e)
            {
                var errMsg = "Failed to parse test summary.";
                WriteExceptionConsoleErrorMessage(errMsg, e);
                throw;
            }
        }

        private XDocument TryLoadResultXmlFile(string resultXmlFileName)
        {
            try
            {
                return XDocument.Load(resultXmlFileName);
            }
            catch (Exception e)
            {
                var errMsg = string.Format("Failed to load xml result file: {0}", resultXmlFileName);
                WriteExceptionConsoleErrorMessage(errMsg, e);
                throw;
            }
        }

        private void WriteExceptionConsoleErrorMessage(string errMsg, Exception e)
        {
            Console.Error.WriteLine("{0}\r\nException: {1}\r\nInnerException: {2}", errMsg, e.Message,
                e.InnerException.Message);
        }
    }
}
