import { LoadingState } from '../loading'

export type LabelFilter = {
  name: string
  value: string
}

export type Filter = {
  levels: LogLevel[]
  from: string
  to: string
  page: number
  pageSize: number
  message: string
  labels: LabelFilter[]
}

export type TableSettings = {
  wide: boolean
  autoSubmitDelay: number
  columns: string[]
}

export type SearchState = Record<string, BucketSearchState>

export type BucketSearchState = {
  filter: Filter
  logs: LogRecord[]
  settings: TableSettings
} & LoadingState

export enum LogLevel {
  Debug = 'Debug',
  Verbose = 'Verbose',
  Info = 'Info',
  Warning = 'Warning',
  Error = 'Error',
  Critical = 'Critical',
  Fatal = 'Fatal',
  Unknown = 'Unknown'
}

export type LogRecord = {
  level: LogLevel
  timestamp: Date
  message: string
  labels: Record<string, string>
}
