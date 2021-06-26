import { ActionType, createAction } from 'typesafe-actions'
import { createLoadingActions } from '../loading/actions'
import { Filter, LogRecord, TableSettings } from './types'

type BucketScoped<T = {}> = T & { bucket: string }

export const SearchActions = {
  resetFilter: createAction('SEARCH/RESET_FILTER')<BucketScoped>(),
  setFilter: createAction('SEARCH/SET_FILTER')<
    BucketScoped<{
      filter: Filter
    }>
  >(),
  load: createAction('SEARCH/LOAD')<BucketScoped>(),
  setLogs: createAction('SEARCH/SET_LOGS')<BucketScoped<{ logs: LogRecord[] }>>(),
  resetLogs: createAction('SEARCH/RESET_LOGS')<BucketScoped>(),
  setSettings: createAction('SEARCH/SET_SETTINGS')<BucketScoped<{ settings: TableSettings }>>(),
  ...createLoadingActions('SEARCH')
}

export type SearchActions = ActionType<typeof SearchActions>
