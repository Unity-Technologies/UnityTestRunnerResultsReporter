using System;
using System.Collections.Generic;
using System.Text;

namespace UnityTestRunnerResultsReporter.Entities
{
    [Serializable]
    public class TestCase
    {
        public TestData data = new TestData();
        public long time;
        public string fixture;
        public string name;
        public int state;
        public string message;
        public long durationMicroseconds;
        public string stackTrace = null;
        public string className = null;
        public List<string> artifacts = new List<string>();
        public List<string> errors = new List<string>();
    }
}
