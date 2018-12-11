using System;
using System.Collections.Generic;
using System.Text;

namespace UnityTestRunnerResultsReporter.Entities
{
    [Serializable]
    public class TestSummary
    {
        public bool success;
        public int time = 0;
        public int inconclusiveCount;
        public int testsCount;
        public int errorCount;
        public int ignoredCount;
        public int notRunCount;
        public int failedCount;
        public int skippedCount;
        public int successCount;
        public int suitesCount = 1;
        public int compilationErrorsCount;
        public int otherErrorsCount;
        public int failureConclusion;
    }
}
