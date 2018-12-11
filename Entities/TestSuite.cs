using System;
using System.Collections.Generic;
using System.Text;

namespace UnityTestRunnerResultsReporter.Entities
{
    [Serializable]
    public class TestSuite
    {
        public string name;
        public List<string> artifacts;
        public List<TestCase> tests;
        public TestRunSummary summary;
        public List<string> minimalCommandLine;
        public bool supportsFilters = true;
    }
}
