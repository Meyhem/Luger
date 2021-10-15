using System;
using Luger.Features.Summary;

namespace Luger.Endpoints.Models
{
    public class BucketSummaryDto
    {
        public uint TraceCount { get; set; }
        public uint DebugCount { get; set; }
        public uint InformationCount { get; set; }
        public uint WarningCount { get; set; }
        public uint ErrorCount { get; set; }
        public uint CriticalCount { get; set; }
        public uint NoneCount { get; set; }
        public uint TotalCount { get; set; }
        public long SampleSize { get; set; }
        public long BucketSize { get; set; }
        public double CalculatedFromTimespanSeconds { get; set; }

        public static BucketSummaryDto From(BucketSummary s)
        {
            return new BucketSummaryDto
            {
                TraceCount = s.TraceCount,
                DebugCount = s.DebugCount,
                InformationCount = s.InformationCount,
                WarningCount = s.WarningCount,
                ErrorCount = s.ErrorCount,
                CriticalCount = s.CriticalCount,
                NoneCount = s.NoneCount,
                TotalCount = s.TotalCount,
                SampleSize = s.SampleSize,
                BucketSize = s.BucketSize,
                CalculatedFromTimespanSeconds = s.CalculatedFromTimespan.TotalSeconds
            };
        }
    }
}