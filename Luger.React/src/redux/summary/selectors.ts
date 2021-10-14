import { createSelector } from 'reselect'
import { RootState } from '../types'

const selectState = (state: RootState) => state.summary

export const selectAllSummaries = createSelector(selectState, state => state)
