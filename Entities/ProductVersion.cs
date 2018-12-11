using System;

namespace UnityTestRunnerResultsReporter.Entities
{
    [Serializable]
    public class ProductVersion
    {
        public string unity_version;
        public string revision;
        public long revisionNumber;
        public string branch;
        public string vcs = "mercurial";
    }
}
