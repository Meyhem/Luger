import dayjs from 'dayjs'
import _ from 'lodash'
import { getType, Reducer } from 'typesafe-actions'
import { SearchActions } from './actions'
import { BucketSearchState, SearchState } from './types'

export const initialState: SearchState = {}

export const bucketInitialState: BucketSearchState = {
  filter: {
    from: dayjs().subtract(15, 'minutes').toJSON(),
    to: new Date().toJSON(),
    levels: [],
    page: 0,
    pageSize: 50,
    message: '',
    labels: [],
    autoreloadSeconds: 0
  },
  settings: {
    columns: ['level', 'timestamp', 'labels', 'message'],
    autoSubmitDelay: 1500,
    wide: true
  },
  loading: false,
  logs: []
}

function setBucketState(state: SearchState, bucket: string, bucketState: Partial<BucketSearchState>): SearchState {
  return { ...state, [bucket]: { ...state[bucket], ...bucketState } }
}

export const searchReducer: Reducer<SearchState, SearchActions> = (state = initialState, action) => {
  switch (action.type) {
    case getType(SearchActions.resetFilter):
      return setBucketState(state, action.payload.bucket, bucketInitialState)
    case getType(SearchActions.setFilter):
      const bucketFilter = state[action.payload.bucket].filter
      // If ONLY page has been incremented then keep cursor for fast search
      if (
        bucketFilter?.page === action.payload.filter.page - 1 &&
        _.isEqual(_.omit(bucketFilter, 'page'), _.omit(action.payload.filter, 'page'))
      ) {
        return setBucketState(state, action.payload.bucket, { filter: { ...action.payload.filter } })
      }

      return setBucketState(state, action.payload.bucket, { filter: { ...action.payload.filter }, cursor: undefined })
    case getType(SearchActions.addLabelFilter):
      return setBucketState(state, action.payload.bucket, {
        filter: {
          ...state[action.payload.bucket].filter,
          labels: [...state[action.payload.bucket].filter.labels, action.payload]
        },
        cursor: undefined
      })

    case getType(SearchActions.setCursor):
      return setBucketState(state, action.payload.bucket, { cursor: action.payload.cursor })
    case getType(SearchActions.setLogs):
      return setBucketState(state, action.payload.bucket, {
        logs: action.payload.logs
      })
    case getType(SearchActions.resetLogs):
      return setBucketState(state, action.payload.bucket, {
        logs: []
      })
    case getType(SearchActions.setSettings):
      return setBucketState(state, action.payload.bucket, { settings: action.payload.settings })
    default:
      return state
  }
}
