using System;
using System.Collections.Generic;
using System.Text;

namespace UnityTestRunnerResultsReporter.Entities
{
    [Serializable]
    public class TestRunSummary
    {
        public List<string> failureReasons = new List<string>();
        public int compilationErrorsCount;
        public long time;
        public int inconclusiveCount;
        public int testsCount;
        public int errorCount;
        public int ignoredCount;
        public int notRunCount;
        public int failedCount;
        public int skippedCount;
        public int successCount;
        public int result;
    }
}
