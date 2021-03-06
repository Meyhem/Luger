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
  autoreloadSeconds: number
}

export type TableSettings = {
  wide: boolean
  autoSubmitDelay: number
  columns: string[]
}

export type Cursor = {
  shard: string
  offset: number
}

export type SearchState = Record<string, BucketSearchState>

export type BucketSearchState = {
  filter: Filter
  cursor?: Cursor
  logs: LogRecord[]
  settings: TableSettings
} & LoadingState

export enum LogLevel {
  Trace = 'Trace',
  Debug = 'Debug',
  Information = 'Information',
  Warning = 'Warning',
  Error = 'Error',
  Critical = 'Critical',
  None = 'None'
}

export type LogRecord = {
  level: LogLevel
  timestamp: Date
  message: string
  labels: Record<string, string>
}
