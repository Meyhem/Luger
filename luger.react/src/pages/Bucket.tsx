import React, { FC, useEffect } from 'react'
import { useDispatch } from 'react-redux'
import { useParams } from 'react-router'
import styled from 'styled-components/macro'
import { Layout } from '../components/Layout'
import { LogFilter } from '../components/LogFilter'
import { LogTable } from '../components/LogTable'
import { LogTableSettings } from '../components/LogTableSettings'
import { SearchActions } from '../redux/search'

type BucketProps = {}

const ScrollContainer = styled.div`
  overflow-x: auto;
  max-width: 100%;
`

export const BucketPage: FC<BucketProps> = ({}) => {
  const { bucket } = useParams<{ bucket: string }>()
  const d = useDispatch()

  // const filters = useSelector((state: RootState) => selectFilter(state, bucket))

  useEffect(() => {
    d(SearchActions.resetLogs({ bucket: bucket }))
    d(SearchActions.load({ bucket: bucket }))
  }, [bucket, d])

  return (
    <Layout heading={bucket}>
      <LogTableSettings bucket={bucket} />
      <LogFilter bucket={bucket} />

      <ScrollContainer>
        <LogTable bucket={bucket} />
      </ScrollContainer>
    </Layout>
  )
}
