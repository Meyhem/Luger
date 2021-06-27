import _ from 'lodash'
import { all, put, select, takeLatest } from 'redux-saga/effects'
import { getType } from 'typesafe-actions'
import { apiCall } from '../../utils/api'
import { AxiosApiResponse } from '../../utils/types'
import { SearchActions } from './actions'
import { selectFilter } from './selectors'
import { Filter, LogLevel, LogRecord } from './types'

export type LogRecordResponse = {
  level: string
  timestamp: string
  message: string
  labels: Record<string, string>
}

export function mapLogRecordResponse(logs: LogRecordResponse[]): LogRecord[] {
  return _.map(logs, l => ({
    level: _.includes(_.values(LogLevel), l.level) ? (l.level as LogLevel) : LogLevel.Unknown,
    timestamp: new Date(l.timestamp),
    message: l.message || '',
    labels: l.labels ?? {}
  }))
}

export function* load({ payload }: ReturnType<typeof SearchActions.load>) {
  const filters: Filter = yield select(state => selectFilter(state, payload.bucket))

  const response: AxiosApiResponse<{ logs: LogRecordResponse[] }> = yield apiCall({
    method: 'post',
    url: `/api/search/${payload.bucket}`,
    data: { ...filters, labels: _.filter(filters.labels, 'name') }
  })

  yield put(SearchActions.setLogs({ bucket: payload.bucket, logs: mapLogRecordResponse(response.data.data.logs) }))
}

export function* reload({ payload }: ReturnType<typeof SearchActions.setFilter>) {
  yield put(SearchActions.setLogs({ bucket: payload.bucket, logs: [] }))
  yield load(SearchActions.load({ bucket: payload.bucket }))
}

export function* searchSaga() {
  yield all([
    takeLatest(getType(SearchActions.load), load),
    takeLatest([getType(SearchActions.setFilter), getType(SearchActions.addLabelFilter)], reload)
  ])
}
