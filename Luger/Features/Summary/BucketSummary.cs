using System;

namespace Luger.Features.Summary
{
    public class BucketSummary
    {
        public uint TraceCount { get; set; }
        public uint DebugCount { get; set; }
        public uint InformationCount { get; set; }
        public uint WarningCount { get; set; }
        public uint ErrorCount { get; set; }
        public uint CriticalCount { get; set; }
        public uint NoneCount { get; set; }

        public uint LogCount => TraceCount + 
                                DebugCount + 
                                InformationCount + 
                                WarningCount + 
                                ErrorCount + 
                                CriticalCount +
                                NoneCount;

        public long BucketSize { get; set; }
        public TimeSpan CalculatedFromTimespan { get; set; }

    }
}