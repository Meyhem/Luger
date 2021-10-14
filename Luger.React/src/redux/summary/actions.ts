import { ActionType, createAction } from 'typesafe-actions'
import { BucketSummary } from '.'

export const SummaryActions = {
  loadBucketSummary: createAction('SUMMARY/load-bucket-summary')<{ bucket: string }>(),
  setBucketSummary: createAction('SUMMARY/set-bucket-summary')<{ bucket: string; summary: BucketSummary }>()
}

export type SummaryActions = ActionType<typeof SummaryActions>
