import _ from 'lodash'
import { generatePath } from 'react-router'
import { Link } from 'react-router-dom'
import styled, { css } from 'styled-components/macro'
import { BucketSummary } from '../../redux/summary'
import { themeColor } from '../../theme'
import { formatApproxByteSize, formatApproxTimespan } from '../../utils/math'
import { Routes } from '../../utils/routes'

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
  box-shadow: 2px 2px 3px 0px #000000;
  border-radius: 4px;
`

const Title = styled.div`
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;

  margin-bottom: 12px;
  font-size: 24px;
`

const SubTitle = styled.i``

const SummaryTable = styled.table`
  width: 100%;
  thead th {
    width: calc(100% / 3);
    text-align: left;
    color: ${themeColor('primary')};
  }

  tbody tr {
    td:first-child {
      padding-left: 12px;
      color: ${themeColor('primary')};
    }

    &:not(:last-child) {
      border-bottom: 1px solid #ddd;
    }
  }
`

const SummaryItem = styled.div`
  display: flex;
  width: 100%;
`
const summaryItemCss = css`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 50%;
  padding: 15px;
`

const SummaryItemName = styled.div`
  ${summaryItemCss};
  font-weight: bold;
`

const SummaryItemValue = styled.div`
  ${summaryItemCss};
  font-size: 16px;
`

function calcRatio(count: number, total: number) {
  if (total === 0) return 'n/a'

  return _.round(count / total, 2)
}

export const BucketSummaryBlock = ({ bucket, summary }: BucketSummaryBlockProps) => {
  return (
    <Container>
      <Title>
        <Link to={generatePath(Routes.Bucket, { bucket })}>
          <b>{bucket}</b>
        </Link>
      </Title>
      <SubTitle>
        {summary.sampleSize === 0 ? (
          <>No data</>
        ) : (
          <>
            Sample of {summary.totalCount} over period of {formatApproxTimespan(summary.calculatedFromTimespanSeconds)}
          </>
        )}
      </SubTitle>
      <SummaryTable>
        <thead>
          <tr>
            <th></th>
            <th>Count</th>
            <th>Rate</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>
              <b>Trace</b>
            </td>
            <td>{summary.traceCount}</td>
            <td>{calcRatio(summary.traceCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>
              <b>Debug</b>
            </td>
            <td>{summary.debugCount}</td>
            <td>{calcRatio(summary.debugCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>
              <b>Information</b>
            </td>
            <td>{summary.informationCount}</td>
            <td>{calcRatio(summary.informationCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>
              <b>Warning</b>
            </td>
            <td>{summary.warningCount}</td>
            <td>{calcRatio(summary.warningCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>
              <b>Error</b>
            </td>
            <td>{summary.errorCount}</td>
            <td>{calcRatio(summary.errorCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>
              <b>Critical</b>
            </td>
            <td>{summary.criticalCount}</td>
            <td>{calcRatio(summary.criticalCount, summary.totalCount)}</td>
          </tr>
          <tr>
            <td>
              <b>None</b>
            </td>
            <td>{summary.noneCount}</td>
            <td>{calcRatio(summary.noneCount, summary.totalCount)}</td>
          </tr>
        </tbody>
      </SummaryTable>

      <SummaryItem>
        <SummaryItemName>Used space</SummaryItemName>
        <SummaryItemValue>{formatApproxByteSize(summary.bucketSize)}</SummaryItemValue>
      </SummaryItem>
    </Container>
  )
}
