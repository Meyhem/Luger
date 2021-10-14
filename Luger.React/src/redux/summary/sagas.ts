import { AxiosResponse } from 'axios'
import { all, put, takeEvery } from 'redux-saga/effects'
import { getType } from 'typesafe-actions'
import { apiCall } from '../../utils/api'
import { SummaryActions } from './actions'

export function* placeholderSaga({ payload }: ReturnType<typeof SummaryActions.loadBucketSummary>) {
  const response: AxiosResponse<{}> = yield apiCall({
    method: 'get',
    url: `/api/summary/${payload.bucket}`
  })

  yield put(SummaryActions.setBucketSummary({ bucket: payload.bucket, summary: response.data as any }))
}

export function* summarySaga() {
  yield all([takeEvery(getType(SummaryActions.loadBucketSummary), placeholderSaga)])
}
