import _ from 'lodash'
import { useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { BucketSummaryBlock } from '../components/BucketSummaryBlock'
import { Flex } from '../components/FlexBox'
import { Layout } from '../components/Layout'
import { getBuckets } from '../redux/auth/selectors'
import { selectAllSummaries, SummaryActions } from '../redux/summary'

export function DashboardPage() {
  const buckets = useSelector(getBuckets)
  const summaryMap = useSelector(selectAllSummaries)
  const d = useDispatch()

  useEffect(() => {
    _.forEach(buckets, b => {
      d(SummaryActions.loadBucketSummary({ bucket: b }))
    })
  }, [buckets, d])

  return (
    <Layout>
      <Flex>
        {_.map(summaryMap, ([bucket, bucketSummary]) => (
          <Flex width={1 / 3} key={bucket}>
            <BucketSummaryBlock bucket={bucket} summary={bucketSummary} />
          </Flex>
        ))}
      </Flex>
    </Layout>
  )
}
