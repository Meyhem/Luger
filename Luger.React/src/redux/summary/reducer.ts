import { getType, Reducer } from 'typesafe-actions'
import { SummaryActions } from './actions'
import { BucketSummary, SummaryState } from './types'

export const initialState: SummaryState = {}

function updateBucketSummary(state: SummaryState, bucket: string, summary: BucketSummary) {
  return {
    ...state,
    [bucket]: summary
  }
}

export const summaryReducer: Reducer<SummaryState, SummaryActions> = (state = initialState, action) => {
  switch (action.type) {
    case getType(SummaryActions.setBucketSummary):
      return updateBucketSummary(state, action.payload.bucket, action.payload.summary)
    default:
      return state
  }
}
