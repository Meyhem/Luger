export type BucketSummary = {
  traceCount: number
  debugCount: number
  informationCount: number
  warningCount: number
  errorCount: number
  criticalCount: number
  noneCount: number
  totalCount: number
  sampleSize: number
  bucketSize: number
  calculatedFromTimespanSeconds: number
}

export type SummaryState = Record<string, BucketSummary>
