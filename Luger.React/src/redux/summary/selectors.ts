import _ from 'lodash'
import { createSelector } from 'reselect'
import { RootState } from '../types'

const selectState = (state: RootState) => state.summary

export const selectAllSummaries = createSelector(selectState, state => _.sortBy(_.toPairs(state), 0))
