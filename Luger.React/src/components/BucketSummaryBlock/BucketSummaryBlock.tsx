import _ from 'lodash'
import styled from 'styled-components/macro'
import { BucketSummary } from '../../redux/summary'

type BucketSummaryBlockProps = {
  bucket: string
  summary: BucketSummary
}

const Container = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  border: 1px solid #aaaaaa;
  margin: 6px;
`

const Heading = styled.div`
  width: 100%;
  display: flex;
  justify-content: center;

  font-size: 24px;
  font-weight: bold;
`

const SummaryTable = styled.table`
  width: 100%;
  thead th {
    width: calc(100% / 3);
    text-align: left;
  }

  tbody tr:not(:last-child) {
    border-bottom: 1px solid #ddd;
  }
`

function calcRatio(count: number, total: number) {
  if (total === 0) return 'n/a'

  return _.round(count / total, 2)
}

export const BucketSummaryBlock = ({ bucket, summary }: BucketSummaryBlockProps) => {
  return (
    <Container>
      <Heading>{bucket}</Heading>
      <SummaryTable>
        <thead>
          <tr>
            <th></th> <th>Sample of {summary.sampleSize}</th> <th>Rate</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>Trace</td> <td>{summary.traceCount}</td> <td>{calcRatio(summary.traceCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>Debug</td> <td>{summary.debugCount}</td> <td>{calcRatio(summary.debugCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>Information</td> <td>{summary.informationCount}</td>{' '}
            <td>{calcRatio(summary.informationCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>Warning</td> <td>{summary.warningCount}</td>{' '}
            <td>{calcRatio(summary.warningCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>Error</td> <td>{summary.errorCount}</td> <td>{calcRatio(summary.errorCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>Critical</td> <td>{summary.criticalCount}</td>{' '}
            <td>{calcRatio(summary.criticalCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>None</td> <td>{summary.noneCount}</td> <td>{calcRatio(summary.noneCount, summary.totalCount)}</td>
          </tr>
        </tbody>
      </SummaryTable>
    </Container>
  )
}
