using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace UnityTestRunnerResultsReporter.Entities
{
    [Serializable]
    public class UnityTestRunnerTestResult
    {
        [JsonProperty(PropertyName = "product-version")]
        public ProductVersion productVersion;

        [JsonProperty(PropertyName = "run-test-session-data")]
        public TestArguments arguments;

        public TestSummary summary;

        public string utrPrefix = "Unity.exe";

        public List<TestSuite> suites; 
    }
}
