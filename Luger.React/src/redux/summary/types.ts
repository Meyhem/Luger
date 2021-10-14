export type BucketSummary = {
  traceCount: number
  debugCount: number
  informationCount: number
  warningCount: number
  errorCount: number
  criticalCount: number
  noneCount: number
  logCount: number
  bucketSize: number
  calculatedFromTimespanSeconds: number
}

export type SummaryState = Record<string, BucketSummary>
