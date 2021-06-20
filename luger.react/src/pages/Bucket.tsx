import React, { FC, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { useParams } from 'react-router'
import { Layout } from '../components/Layout'
import { LogFilter } from '../components/LogFilter'
import { LogTable } from '../components/LogTable'
import { SearchActions } from '../redux/search'
import { selectLogs } from '../redux/search/selectors'
import { RootState } from '../redux/types'

type BucketProps = {}

export const BucketPage: FC<BucketProps> = ({}) => {
  const { bucket } = useParams<{ bucket: string }>()
  const d = useDispatch()

  // const filters = useSelector((state: RootState) => selectFilter(state, bucket))
  const logs = useSelector((state: RootState) => selectLogs(state, bucket))

  useEffect(() => {
    d(SearchActions.resetLogs({ bucket: bucket }))
    d(SearchActions.load({ bucket: bucket }))
  }, [bucket, d])

  return (
    <Layout heading={bucket}>
      <LogFilter bucket={bucket} />
      <LogTable records={logs} />
    </Layout>
  )
}
